using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.Mensajeria.Eventos;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class PedidoInternoService : IPedIntService
    {
        private readonly PedIntRepositorio _pedidoInternoRepositorio;
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly DepartaRepositorio _departamentoRepositorio;
        private readonly SucursalRepositorio _sucursalRepositorio;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public PedidoInternoService(
            PedIntRepositorio pedidoInternoRepository,
            OrdComRepositorio ordenCompraRepositorio,
            DepartaRepositorio departamentoRepositorio,
            SucursalRepositorio sucursalRepositorio,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _pedidoInternoRepositorio = pedidoInternoRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;
            _departamentoRepositorio = departamentoRepositorio;
            _sucursalRepositorio = sucursalRepositorio;
            _publishEndpoint = publishEndpoint;
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

            var todos = await _pedidoInternoRepositorio.GetAllasync();
            var nuevoPedido = _mapper.Map<Pedido_Interno>(pedido);
            nuevoPedido.id = todos.Any() ? todos.Max(p => p.id) + 1 : 1;

            if (string.IsNullOrEmpty(nuevoPedido.codigo))
            {
                var anio = DateTime.Now.Year;
                nuevoPedido.codigo = $"PED-{anio}-{nuevoPedido.id.ToString("D3")}";
            }

            if (!nuevoPedido.cantidad.HasValue)
            {
                nuevoPedido.cantidad = 1;
            }

            await _pedidoInternoRepositorio.AddAsync(nuevoPedido);

            await _publishEndpoint.Publish(new PedidoCreado(
                PedidoId: nuevoPedido.id,
                Codigo: nuevoPedido.codigo ?? "",
                IdDepartamento: nuevoPedido.id_Departamento,
                Cantidad: nuevoPedido.cantidad ?? 1,
                Fecha: DateTime.UtcNow,
                Usuario: "Sistema"
            ));
        }

        public async Task DeleteAsync(int id)
        {
            var pedido = await _pedidoInternoRepositorio.GetAsync(id);
            if (pedido != null)
            {
                await _pedidoInternoRepositorio.DeletAsync(id);

                await _publishEndpoint.Publish(new PedidoCancelado(
                    PedidoId: id,
                    Codigo: pedido.codigo ?? "",
                    Motivo: "Cancelado por el usuario",
                    Fecha: DateTime.UtcNow,
                    Usuario: "Sistema"
                ));
            }
        }

        public async Task<List<PedidoInternoDTO>> GetAllsync()
        {
            var pedidos = await _pedidoInternoRepositorio.GetAllasync();
            var departamentos = await _departamentoRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();

            return pedidos.Select(p =>
            {
                var depto = departamentos.FirstOrDefault(d => d.id == p.id_Departamento);
                var sucursal = depto != null
                    ? sucursales.FirstOrDefault(s => s.id == depto.id_Sucursal)
                    : null;

                return new PedidoInternoDTO(
                    p.id,
                    p.codigo,
                    p.cantidad,
                    p.id_Departamento,
                    depto?.Nombre,
                    p.id_OrdenCompra,
                    p.Fecha_Solicitada,
                    p.Fecha_Ingreso,
                    sucursal?.Nombre,
                    sucursal?.id
                );
            }).ToList();
        }

        public async Task<PedidoInternoDTO> GetByIdAsync(int id)
        {
            var pedido = await _pedidoInternoRepositorio.GetAsync(id);
            if (pedido == null) return null;

            var departamentos = await _departamentoRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();

            var depto = departamentos.FirstOrDefault(d => d.id == pedido.id_Departamento);
            var sucursal = depto != null
                ? sucursales.FirstOrDefault(s => s.id == depto.id_Sucursal)
                : null;

            return new PedidoInternoDTO(
                pedido.id,
                pedido.codigo,
                pedido.cantidad,
                pedido.id_Departamento,
                depto?.Nombre,
                pedido.id_OrdenCompra,
                pedido.Fecha_Solicitada,
                pedido.Fecha_Ingreso,
                sucursal?.Nombre,
                sucursal?.id
            );
        }

        // ===== MÉTODO CORREGIDO: UpdateAsync =====
        public async Task UpdateAsync(UpdatePedidoInternoDTO pedido)
        {
            // ===== OBTENER LA ENTIDAD YA RASTREADA =====
            var pedidoExistente = await _pedidoInternoRepositorio.GetAsync(pedido.id);
            if (pedidoExistente == null)
            {
                throw new InvalidOperationException($"El pedido #{pedido.id} no existe.");
            }

            // ===== VALIDACIÓN: Pedido no puede estar en 2 órdenes =====
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

            // ===== ACTUALIZAR LA ENTIDAD YA RASTREADA (NO mapear) =====
            pedidoExistente.codigo = pedido.codigo;
            pedidoExistente.cantidad = pedido.cantidad ?? 1;
            pedidoExistente.id_Departamento = pedido.id_Departamento;
            pedidoExistente.id_OrdenCompra = pedido.id_OrdenCompra;
            pedidoExistente.Fecha_Solicitada = pedido.Fecha_Solicitada;
            pedidoExistente.Fecha_Ingreso = pedido.Fecha_Ingreso;

            await _pedidoInternoRepositorio.UpdateAsync(pedidoExistente);

            // ===== EVENTO: PedidoActualizado =====
            await _publishEndpoint.Publish(new PedidoActualizado(
                PedidoId: pedidoExistente.id,
                Codigo: pedidoExistente.codigo ?? "",
                Cantidad: pedidoExistente.cantidad ?? 1,
                IdDepartamento: pedidoExistente.id_Departamento,
                Fecha: DateTime.UtcNow,
                Usuario: "Sistema"
            ));
        }

        public async Task<List<PedidoInternoDTO>> GetByDepartamentoAsync(int idDepartamento)
        {
            var todos = await GetAllsync();
            return todos.Where(p => p.id_Departamento == idDepartamento).ToList();
        }
    }
}