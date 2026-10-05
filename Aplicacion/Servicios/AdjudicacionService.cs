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
    public class AdjudicacionService : IAdjudicacionService
    {
        private readonly AdjuRepositorio _adjuRepositorio;
        private readonly OrdComRepositorio _ordenCompraRepositorio;
        private readonly OferProvRepositorio _ofertaRepositorio;
        private readonly DetaAdjRepositorio _detaAdjRepositorio;
        private readonly PedIntRepositorio _pedidoRepositorio;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public AdjudicacionService(
            AdjuRepositorio adjuRepository,
            OrdComRepositorio ordenCompraRepositorio,
            OferProvRepositorio ofertaRepositorio,
            DetaAdjRepositorio detaAdjRepositorio,
            PedIntRepositorio pedidoRepositorio,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _adjuRepositorio = adjuRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;
            _ofertaRepositorio = ofertaRepositorio;
            _detaAdjRepositorio = detaAdjRepositorio;
            _pedidoRepositorio = pedidoRepositorio;
            _publishEndpoint = publishEndpoint;
        }

        // ==================== ADD (CREATE) ====================
        public async Task AddAsync(CreateAdjudicacionDTO adjudicacion)
        {
            var orden = await _ordenCompraRepositorio.GetAsync(adjudicacion.Orden_Compra);

            if (orden == null)
            {
                throw new InvalidOperationException(
                    $"La orden de compra #{adjudicacion.Orden_Compra} no existe."
                );
            }

            if (adjudicacion.Fecha_Resolucion < orden.Fecha_Creacion)
            {
                throw new InvalidOperationException(
                    $"La fecha de resolución ({adjudicacion.Fecha_Resolucion:dd/MM/yyyy}) " +
                    $"no puede ser anterior a la fecha de creación de la orden " +
                    $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                );
            }

            var todas = await _adjuRepositorio.GetAllasync();
            var nuevaAdjudicacion = _mapper.Map<Adjudicacion>(adjudicacion);
            nuevaAdjudicacion.id = todas.Any() ? todas.Max(a => a.id) + 1 : 1;

            await _adjuRepositorio.AddAsync(nuevaAdjudicacion);

            await _publishEndpoint.Publish(new CompraRegistrada(
                OrdenId: Guid.NewGuid(),
                Detalle: $"Adjudicación de la orden #{adjudicacion.Orden_Compra}",
                Monto: 0,
                Fecha: DateTime.UtcNow,
                Proveedor: "Por definir"
            ));

            await _publishEndpoint.Publish(new AdjudicacionCreada(
                AdjudicacionId: nuevaAdjudicacion.id,
                OrdenCompra: nuevaAdjudicacion.Orden_Compra,
                Fecha_Resolucion: nuevaAdjudicacion.Fecha_Resolucion,
                Estado: nuevaAdjudicacion.Estado ?? "Activa",
                Fecha: DateTime.UtcNow,
                Usuario: "Sistema"
            ));
        }

        // ==================== ADJUDICAR MÚLTIPLES PEDIDOS ====================
        public async Task AdjudicarPedidosAsync(AdjudicarPedidosDTO dto)
        {
            if (dto.Pedidos == null || !dto.Pedidos.Any())
            {
                throw new InvalidOperationException(
                    "Debes seleccionar al menos un pedido para adjudicar."
                );
            }

            var orden = await _ordenCompraRepositorio.GetAsync(dto.Orden_Compra);
            if (orden == null)
            {
                throw new InvalidOperationException(
                    $"La orden de compra #{dto.Orden_Compra} no existe."
                );
            }

            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var todosLosDetalles = await _detaAdjRepositorio.GetAllasync();

            // Validar CADA pedido ANTES de crear nada
            foreach (var pedido in dto.Pedidos)
            {
                var detalleExistente = todosLosDetalles.FirstOrDefault(d => d.id_Pedido == pedido.id_Pedido);
                if (detalleExistente != null)
                {
                    throw new InvalidOperationException(
                        $"El pedido #{pedido.id_Pedido} ya está adjudicado en la adjudicación " +
                        $"#{detalleExistente.id_adjudicacion}. Un pedido solo puede tener una única adjudicación."
                    );
                }

                var ofertasDelPedido = todasLasOfertas
                    .Where(o => o.id_Pedido_Interno == pedido.id_Pedido)
                    .ToList();

                if (!ofertasDelPedido.Any())
                {
                    throw new InvalidOperationException(
                        $"No se puede adjudicar el pedido #{pedido.id_Pedido} porque no tiene ofertas registradas."
                    );
                }

                var menorPrecio = ofertasDelPedido.Min(o => o.Precio);
                var proveedorEsGanador = ofertasDelPedido
                    .Any(o => o.id_Proveedor == pedido.id_Proveedor && o.Precio == menorPrecio);

                if (!proveedorEsGanador)
                {
                    var proveedoresMenor = string.Join(", ",
                        ofertasDelPedido
                            .Where(o => o.Precio == menorPrecio)
                            .Select(o => $"#{o.id_Proveedor}"));

                    throw new InvalidOperationException(
                        $"La adjudicación del pedido #{pedido.id_Pedido} debe ir al proveedor con menor precio " +
                        $"(Q {menorPrecio:N2} de los proveedores {proveedoresMenor})."
                    );
                }

                var pedidoInterno = await _pedidoRepositorio.GetAsync(pedido.id_Pedido);
                if (pedidoInterno == null)
                {
                    throw new InvalidOperationException(
                        $"El pedido interno #{pedido.id_Pedido} no existe."
                    );
                }
            }

            // ===== CREAR LA ADJUDICACIÓN (CABECERA) CON FECHA Y ESTADO AUTOMÁTICOS =====
            var pedidosDeLaOrden = await _pedidoRepositorio.GetAllasync();
            var pedidosDeLaOrdenFiltrados = pedidosDeLaOrden
                .Where(p => p.id_OrdenCompra == dto.Orden_Compra)
                .ToList();

            var pedidosYaAdjudicados = todosLosDetalles
                .Select(d => d.id_Pedido)
                .ToHashSet();

            var pedidosNuevos = dto.Pedidos
                .Select(p => p.id_Pedido)
                .ToHashSet();

            var todosAdjudicados = pedidosDeLaOrdenFiltrados.All(p =>
                pedidosYaAdjudicados.Contains(p.id) || pedidosNuevos.Contains(p.id)
            );

            string estadoFinal;
            if (todosAdjudicados && pedidosDeLaOrdenFiltrados.Any())
            {
                estadoFinal = "Completada";
            }
            else
            {
                estadoFinal = "Activa";
            }

            var todasLasAdjudicaciones = await _adjuRepositorio.GetAllasync();

            // ✅ Calcular el siguiente ID disponible de forma segura
            int nuevoIdAdjudicacion = 1;
            if (todasLasAdjudicaciones.Any())
            {
                nuevoIdAdjudicacion = todasLasAdjudicaciones.Max(a => a.id) + 1;
                var idsExistentes = todasLasAdjudicaciones.Select(a => a.id).ToHashSet();
                while (idsExistentes.Contains(nuevoIdAdjudicacion))
                {
                    nuevoIdAdjudicacion++;
                }
            }

            var nuevaAdjudicacion = new Adjudicacion
            {
                id = nuevoIdAdjudicacion,
                Fecha_Resolucion = DateTime.Now,
                Orden_Compra = dto.Orden_Compra,
                Estado = estadoFinal
            };

            await _adjuRepositorio.AddAsync(nuevaAdjudicacion); // ← Solo UNA vez

            // ===== CREAR LOS DETALLES (UNO POR CADA PEDIDO) =====
            foreach (var pedido in dto.Pedidos)
            {
                var ofertaGanadora = todasLasOfertas
                    .First(o => o.id_Pedido_Interno == pedido.id_Pedido
                             && o.id_Proveedor == pedido.id_Proveedor);

                var pedidoInterno = await _pedidoRepositorio.GetAsync(pedido.id_Pedido);

                var detalle = new Detalle_Adjudicacion
                {
                    id_adjudicacion = nuevaAdjudicacion.id,
                    id_Pedido = pedido.id_Pedido,
                    id_Proveedor = pedido.id_Proveedor,
                    Precio = ofertaGanadora.Precio,
                    Cantidad = pedidoInterno.cantidad ?? 1
                };

                await _detaAdjRepositorio.AddAsync(detalle);
            }

            // ===== ✅ ACTUALIZAR ESTADO DE LA ORDEN SI TODOS LOS PEDIDOS ESTÁN ADJUDICADOS =====
            var todosLosDetallesFinal = await _detaAdjRepositorio.GetAllasync();
            var idsPedidosAdjudicadosFinal = todosLosDetallesFinal.Select(d => d.id_Pedido).ToHashSet();
            var pedidosDeLaOrdenFinal = (await _pedidoRepositorio.GetAllasync())
                .Where(p => p.id_OrdenCompra == dto.Orden_Compra)
                .ToList();

            var todosAdjudicadosFinal = pedidosDeLaOrdenFinal.Any()
                && pedidosDeLaOrdenFinal.All(p => idsPedidosAdjudicadosFinal.Contains(p.id));

            if (todosAdjudicadosFinal)
            {
                var ordenActualizar = await _ordenCompraRepositorio.GetAsync(dto.Orden_Compra);
                if (ordenActualizar != null && ordenActualizar.Estado != "Adjudicada")
                {
                    ordenActualizar.Estado = "Adjudicada";
                    await _ordenCompraRepositorio.UpdateAsync(ordenActualizar);
                }
            }

            // ===== PUBLICAR EVENTOS =====
            await _publishEndpoint.Publish(new CompraRegistrada(
                OrdenId: Guid.NewGuid(),
                Detalle: $"Adjudicación de la orden #{dto.Orden_Compra} ({dto.Pedidos.Count} pedidos)",
                Monto: 0,
                Fecha: DateTime.UtcNow,
                Proveedor: "Por definir"
            ));

            await _publishEndpoint.Publish(new AdjudicacionCreada(
                AdjudicacionId: nuevaAdjudicacion.id,
                OrdenCompra: nuevaAdjudicacion.Orden_Compra,
                Fecha_Resolucion: nuevaAdjudicacion.Fecha_Resolucion,
                Estado: nuevaAdjudicacion.Estado ?? "Activa",
                Fecha: DateTime.UtcNow,
                Usuario: "Sistema"
            ));
        }

        public async Task DeleteAsync(int id)
        {
            await _adjuRepositorio.DeletAsync(id);
        }

        public async Task<List<AdjudicacionDTO>> GetAllsync()
        {
            return _mapper.Map<List<AdjudicacionDTO>>(await _adjuRepositorio.GetAllasync());
        }

        public async Task<AdjudicacionDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<AdjudicacionDTO>(await _adjuRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateAdjudicacionDTO adjudicacion)
        {
            var orden = await _ordenCompraRepositorio.GetAsync(adjudicacion.Orden_Compra);

            if (orden == null)
            {
                throw new InvalidOperationException(
                    $"La orden de compra #{adjudicacion.Orden_Compra} no existe."
                );
            }

            if (adjudicacion.Fecha_Resolucion < orden.Fecha_Creacion)
            {
                throw new InvalidOperationException(
                    $"La fecha de resolución ({adjudicacion.Fecha_Resolucion:dd/MM/yyyy}) " +
                    $"no puede ser anterior a la fecha de creación de la orden " +
                    $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                );
            }

            await _adjuRepositorio.UpdateAsync(_mapper.Map<Adjudicacion>(adjudicacion));
        }

        public async Task ValidarMenorPrecioAsync(int idPedido, int idProveedor, decimal precioAdjudicado)
        {
            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var ofertasDelPedido = todasLasOfertas
                .Where(o => o.id_Pedido_Interno == idPedido)
                .ToList();

            if (!ofertasDelPedido.Any())
            {
                throw new InvalidOperationException(
                    $"No se puede adjudicar el pedido #{idPedido} porque no tiene ofertas registradas."
                );
            }

            var mejorOferta = ofertasDelPedido
                .OrderBy(o => o.Precio)
                .First();

            var ofertasConMenorPrecio = ofertasDelPedido
                .Where(o => o.Precio == mejorOferta.Precio)
                .ToList();

            var proveedorEsGanador = ofertasConMenorPrecio
                .Any(o => o.id_Proveedor == idProveedor);

            if (!proveedorEsGanador)
            {
                var proveedoresMenor = string.Join(", ",
                    ofertasConMenorPrecio.Select(o => $"#{o.id_Proveedor}"));

                throw new InvalidOperationException(
                    $"La adjudicación del pedido #{idPedido} debe ir al proveedor con menor precio " +
                    $"(Q {mejorOferta.Precio:N2} del proveedor {proveedoresMenor}). " +
                    $"El proveedor seleccionado (#{idProveedor}) ofertó Q {precioAdjudicado:N2}."
                );
            }
        }
    }
}