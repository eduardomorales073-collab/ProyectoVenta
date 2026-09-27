using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class PedidoInternoService : IPedIntService
    {
        private readonly PedIntRepositorio _pedidoInternoRepositorio;
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly IMapper _mapper;

        public PedidoInternoService(
            PedIntRepositorio pedidoInternoRepository,
            OrdComRepositorio ordenCompraRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _pedidoInternoRepositorio = pedidoInternoRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;
        }

        public async Task AddAsync(CreatePedidoInternoDTO pedido)
        {
            // ===== REGLA DE NEGOCIO 1 =====
            if (pedido.id_OrdenCompra.HasValue)
            {
                var orden = await _ordenCompraRepositorio.GetAsync(pedido.id_OrdenCompra.Value);

                if (orden == null)
                {
                    throw new InvalidOperationException(
                        $"La orden de compra #{pedido.id_OrdenCompra.Value} no existe."
                    );
                }

                if (pedido.Fecha_Solicitada > orden.Fecha_Creacion)
                {
                    throw new InvalidOperationException(
                        $"La fecha de solicitud del pedido ({pedido.Fecha_Solicitada:dd/MM/yyyy}) " +
                        $"no puede ser posterior a la fecha de creación de la orden " +
                        $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                    );
                }
            }

            // Calcular el id manualmente
            var todos = await _pedidoInternoRepositorio.GetAllasync();
            var nuevoPedido = _mapper.Map<Pedido_Interno>(pedido);
            nuevoPedido.id = todos.Any() ? todos.Max(p => p.id) + 1 : 1;

            // ===== AUTO-GENERAR CÓDIGO =====
            if (string.IsNullOrEmpty(nuevoPedido.codigo))
            {
                var anio = DateTime.Now.Year;
                nuevoPedido.codigo = $"PED-{anio}-{nuevoPedido.id.ToString("D3")}";
            }

            // ===== VALORES POR DEFECTO =====
            if (!nuevoPedido.cantidad.HasValue)
            {
                nuevoPedido.cantidad = 1;
            }

            await _pedidoInternoRepositorio.AddAsync(nuevoPedido);
        }

        public async Task DeleteAsync(int id)
        {
            await _pedidoInternoRepositorio.DeletAsync(id);
        }

        public async Task<List<PedidoInternoDTO>> GetAllsync()
        {
            return _mapper.Map<List<PedidoInternoDTO>>(await _pedidoInternoRepositorio.GetAllasync());
        }

        public async Task<PedidoInternoDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<PedidoInternoDTO>(await _pedidoInternoRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdatePedidoInternoDTO pedido)
        {
            // ===== VALIDACIÓN: Pedido no puede estar en 2 órdenes =====
            var pedidoExistente = await _pedidoInternoRepositorio.GetAsync(pedido.id);
            if (pedidoExistente == null)
            {
                throw new InvalidOperationException($"El pedido #{pedido.id} no existe.");
            }

            // Si el pedido ya tenía una orden asignada y se intenta cambiar a otra
            if (pedidoExistente.id_OrdenCompra.HasValue
                && pedido.id_OrdenCompra.HasValue
                && pedidoExistente.id_OrdenCompra.Value != pedido.id_OrdenCompra.Value)
            {
                throw new InvalidOperationException(
                    $"El pedido #{pedido.id} ya está asignado a la orden #{pedidoExistente.id_OrdenCompra.Value}. " +
                    $"No se puede reasignar a la orden #{pedido.id_OrdenCompra.Value} sin desasignarlo primero."
                );
            }

            // ===== REGLA DE NEGOCIO 1 (fecha) =====
            if (pedido.id_OrdenCompra.HasValue)
            {
                var orden = await _ordenCompraRepositorio.GetAsync(pedido.id_OrdenCompra.Value);

                if (orden == null)
                {
                    throw new InvalidOperationException(
                        $"La orden de compra #{pedido.id_OrdenCompra.Value} no existe."
                    );
                }

                if (pedido.Fecha_Solicitada > orden.Fecha_Creacion)
                {
                    throw new InvalidOperationException(
                        $"La fecha de solicitud del pedido ({pedido.Fecha_Solicitada:dd/MM/yyyy}) " +
                        $"no puede ser posterior a la fecha de creación de la orden " +
                        $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                    );
                }
            }

            // ===== VALORES POR DEFECTO =====
            var pedidoActualizar = _mapper.Map<Pedido_Interno>(pedido);
            if (!pedidoActualizar.cantidad.HasValue)
            {
                pedidoActualizar.cantidad = 1;
            }

            await _pedidoInternoRepositorio.UpdateAsync(pedidoActualizar);
        }
    }
}