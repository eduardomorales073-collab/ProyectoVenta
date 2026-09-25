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
            // ===== REGLA DE NEGOCIO 1 (también al actualizar) =====
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

            await _pedidoInternoRepositorio.UpdateAsync(_mapper.Map<Pedido_Interno>(pedido));
        }
    }
}