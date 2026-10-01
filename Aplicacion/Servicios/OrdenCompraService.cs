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
using Aplicacion.Mensajeria.Eventos;
using MassTransit;

namespace Aplicacion.Servicios
{
    public class OrdenCompraService : IOrdComService
    {
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly PedIntRepositorio _pedidoRepositorio;        
        private readonly DetaAdjRepositorio _detalleAdjRepositorio;    
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public OrdenCompraService(
            OrdComRepositorio ordenCompraRepository,
            PedIntRepositorio pedidoRepositorio,                       
            DetaAdjRepositorio detalleAdjRepositorio,                  
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _ordenCompraRepositorio = ordenCompraRepository;
            _pedidoRepositorio = pedidoRepositorio;                     
            _detalleAdjRepositorio = detalleAdjRepositorio;            
        }

        public async Task AddAsync(CreateOrdenCompraDTO orden)
        {
            var nuevaOrden = _mapper.Map<Orden_Compra>(orden);

            var todos = await _ordenCompraRepositorio.GetAllasync();
            nuevaOrden.id = todos.Any() ? todos.Max(o => o.id) + 1 : 1;

            if (!nuevaOrden.fecha_limite_ofertas.HasValue)
            {
                nuevaOrden.fecha_limite_ofertas = nuevaOrden.Fecha_Limite;
            }

            await _ordenCompraRepositorio.AddAsync(nuevaOrden);

            await _publishEndpoint.Publish(new OrdenCreada(
                OrdenId: nuevaOrden.id,
                Descripcion: nuevaOrden.Descripcion,
                Fecha_Creacion: nuevaOrden.Fecha_Creacion,
                Fecha_Limite: nuevaOrden.Fecha_Limite,
                Tipo_Orden: nuevaOrden.Tipo_Orden
            ));
        }

        public async Task DeleteAsync(int id)
        {
            await _ordenCompraRepositorio.DeletAsync(id);
        }

        public async Task<List<OrdenCompraDTO>> GetAllsync()
        {
            return _mapper.Map<List<OrdenCompraDTO>>(await _ordenCompraRepositorio.GetAllasync());
        }

        public async Task<OrdenCompraDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<OrdenCompraDTO>(await _ordenCompraRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateOrdenCompraDTO orden)
        {
            var ordenActualizar = _mapper.Map<Orden_Compra>(orden);

            if (!ordenActualizar.fecha_limite_ofertas.HasValue)
            {
                ordenActualizar.fecha_limite_ofertas = ordenActualizar.Fecha_Limite;
            }

            await _ordenCompraRepositorio.UpdateAsync(ordenActualizar);
        }

        // ==================== NUEVOS MÉTODOS ====================

        public async Task<List<OrdenCompraConContadoresDTO>> GetConContadoresAsync()
        {
            var ordenes = await _ordenCompraRepositorio.GetAllasync();
            var pedidos = await _pedidoRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();

            return ordenes
                .OrderByDescending(o => o.id)
                .Select(o =>
                {
                    // Pedidos de esta orden
                    var pedidosDeLaOrden = pedidos
                        .Where(p => p.id_OrdenCompra == o.id)
                        .ToList();

                    var totalPedidos = pedidosDeLaOrden.Count;

                    // Pedidos adjudicados (los que tienen detalle de adjudicación)
                    var idsPedidosAdjudicados = detallesAdj
                        .Select(d => d.id_Pedido)
                        .ToHashSet();

                    var pedidosAdjudicados = pedidosDeLaOrden
                        .Count(p => idsPedidosAdjudicados.Contains(p.id));

                    var pedidosPendientes = totalPedidos - pedidosAdjudicados;

                    // Estado: Cerrada si todos los pedidos están adjudicados y hay al menos 1
                    var estado = (totalPedidos > 0 && pedidosPendientes == 0)
                        ? "Cerrada"
                        : "Abierta";

                    return new OrdenCompraConContadoresDTO(
                        o.id,
                        o.Descripcion,
                        o.Fecha_Creacion,
                        o.Fecha_Limite,
                        o.fecha_limite_ofertas,
                        o.Tipo_Orden,
                        totalPedidos,
                        pedidosAdjudicados,
                        pedidosPendientes,
                        estado
                    );
                }).ToList();
        }

        public async Task<List<PedidoInternoDTO>> GetPedidosDeOrdenAsync(int idOrden)
        {
            var pedidos = await _pedidoRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();

            var idsPedidosAdjudicados = detallesAdj
                .Select(d => d.id_Pedido)
                .ToHashSet();

            return pedidos
                .Where(p => p.id_OrdenCompra == idOrden)
                .OrderByDescending(p => p.urgente)
                .ThenByDescending(p => p.id)
                .Select(p => new PedidoInternoDTO(
                    p.id,
                    p.codigo,
                    p.cantidad,
                    p.id_Departamento,
                    null,                              // nombreDepartamento
                    p.id_OrdenCompra,
                    p.Fecha_Solicitada,
                    p.Fecha_Ingreso,
                    null,                              // nombreSucursal
                    null,                              // id_Sucursal
                    p.urgente,
                    0,                                 
                    idsPedidosAdjudicados.Contains(p.id),  // adjudicado
                    null,                              // idProveedorGanador
                    null,                              // nombreProveedorGanador
                    null                               // precioAdjudicado
                )).ToList();
        }
    }
}