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
        private readonly DetaAdjRepositorio _detaAdjRepositorio;      // ← NUEVO
        private readonly PedIntRepositorio _pedidoRepositorio;         // ← NUEVO
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public AdjudicacionService(
            AdjuRepositorio adjuRepository,
            OrdComRepositorio ordenCompraRepositorio,
            OferProvRepositorio ofertaRepositorio,
            DetaAdjRepositorio detaAdjRepositorio,                     // ← NUEVO
            PedIntRepositorio pedidoRepositorio,                       // ← NUEVO
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _adjuRepositorio = adjuRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;
            _ofertaRepositorio = ofertaRepositorio;
            _detaAdjRepositorio = detaAdjRepositorio;                  // ← NUEVO
            _pedidoRepositorio = pedidoRepositorio;                    // ← NUEVO
            _publishEndpoint = publishEndpoint;
        }

        // ==================== ADD (CREATE) ====================
        public async Task AddAsync(CreateAdjudicacionDTO adjudicacion)
        {
            // ===== REGLA DE NEGOCIO 2 =====
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

            // Calcular el id manualmente
            var todas = await _adjuRepositorio.GetAllasync();
            var nuevaAdjudicacion = _mapper.Map<Adjudicacion>(adjudicacion);
            nuevaAdjudicacion.id = todas.Any() ? todas.Max(a => a.id) + 1 : 1;

            await _adjuRepositorio.AddAsync(nuevaAdjudicacion);

            // ===== EVENTO EXISTENTE: CompraRegistrada =====
            await _publishEndpoint.Publish(new CompraRegistrada(
                OrdenId: Guid.NewGuid(),
                Detalle: $"Adjudicación de la orden #{adjudicacion.Orden_Compra}",
                Monto: 0,
                Fecha: DateTime.UtcNow,
                Proveedor: "Por definir"
            ));

            // ===== EVENTO NUEVO: AdjudicacionCreada =====
            await _publishEndpoint.Publish(new AdjudicacionCreada(
                AdjudicacionId: nuevaAdjudicacion.id,
                OrdenCompra: nuevaAdjudicacion.Orden_Compra,
                Fecha_Resolucion: nuevaAdjudicacion.Fecha_Resolucion,
                Estado: nuevaAdjudicacion.Estado ?? "Activa",
                Fecha: DateTime.UtcNow,
                Usuario: "Sistema"
            ));
        }

        // ==================== ADJUDICAR MÚLTIPLES PEDIDOS (ENFOQUE B) ====================
        public async Task AdjudicarPedidosAsync(AdjudicarPedidosDTO dto)
        {
            // ===== VALIDACIÓN 1: Al menos un pedido =====
            if (dto.Pedidos == null || !dto.Pedidos.Any())
            {
                throw new InvalidOperationException(
                    "Debes seleccionar al menos un pedido para adjudicar."
                );
            }

            // ===== VALIDACIÓN 2: La orden existe =====
            var orden = await _ordenCompraRepositorio.GetAsync(dto.Orden_Compra);
            if (orden == null)
            {
                throw new InvalidOperationException(
                    $"La orden de compra #{dto.Orden_Compra} no existe."
                );
            }

            // ===== VALIDACIÓN 3: Todas las ofertas (para validar reglas 4, 5, 6) =====
            var todasLasOfertas = await _ofertaRepositorio.GetAllasync();
            var todosLosDetalles = await _detaAdjRepositorio.GetAllasync();

            // Validar CADA pedido ANTES de crear nada (transacción atómica)
            foreach (var pedido in dto.Pedidos)
            {
                // ----- REGLA 5: Un pedido solo puede tener 1 adjudicación -----
                var detalleExistente = todosLosDetalles.FirstOrDefault(d => d.id_Pedido == pedido.id_Pedido);
                if (detalleExistente != null)
                {
                    throw new InvalidOperationException(
                        $"El pedido #{pedido.id_Pedido} ya está adjudicado en la adjudicación " +
                        $"#{detalleExistente.id_adjudicacion}. Un pedido solo puede tener una única adjudicación."
                    );
                }

                // ----- REGLA 6: No adjudicar sin ofertas -----
                var ofertasDelPedido = todasLasOfertas
                    .Where(o => o.id_Pedido_Interno == pedido.id_Pedido)
                    .ToList();

                if (!ofertasDelPedido.Any())
                {
                    throw new InvalidOperationException(
                        $"No se puede adjudicar el pedido #{pedido.id_Pedido} porque no tiene ofertas registradas."
                    );
                }

                // ----- REGLA 4: Adjudicación al proveedor con MENOR precio (permite empates) -----
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

                // ----- VALIDACIÓN EXTRA: El pedido existe -----
                var pedidoInterno = await _pedidoRepositorio.GetAsync(pedido.id_Pedido);
                if (pedidoInterno == null)
                {
                    throw new InvalidOperationException(
                        $"El pedido interno #{pedido.id_Pedido} no existe."
                    );
                }
            }

            // ===== CREAR LA ADJUDICACIÓN (CABECERA) CON FECHA Y ESTADO AUTOMÁTICOS =====
            var todasLasAdjudicaciones = await _adjuRepositorio.GetAllasync();
            var nuevaAdjudicacion = new Adjudicacion
            {
                id = todasLasAdjudicaciones.Any() ? todasLasAdjudicaciones.Max(a => a.id) + 1 : 1,
                Fecha_Resolucion = DateTime.Now,      // ✅ Fecha automática
                Orden_Compra = dto.Orden_Compra,
                Estado = "Activa"                      // ✅ Estado automático
            };

            await _adjuRepositorio.AddAsync(nuevaAdjudicacion);

            // ===== CREAR LOS DETALLES (UNO POR CADA PEDIDO) =====
            foreach (var pedido in dto.Pedidos)
            {
                // Obtener la oferta ganadora (ya validamos que existe)
                var ofertaGanadora = todasLasOfertas
                    .First(o => o.id_Pedido_Interno == pedido.id_Pedido
                             && o.id_Proveedor == pedido.id_Proveedor);

                // Obtener el pedido para saber la cantidad
                var pedidoInterno = await _pedidoRepositorio.GetAsync(pedido.id_Pedido);

                var detalle = new Detalle_Adjudicacion
                {
                    id_adjudicacion = nuevaAdjudicacion.id,
                    id_Pedido = pedido.id_Pedido,
                    id_Proveedor = pedido.id_Proveedor,
                    Precio = ofertaGanadora.Precio,                // ✅ Precio de la oferta ganadora
                    Cantidad = pedidoInterno.cantidad ?? 1          // ✅ Cantidad del pedido
                };

                await _detaAdjRepositorio.AddAsync(detalle);
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

        // ==================== DELETE ====================
        public async Task DeleteAsync(int id)
        {
            await _adjuRepositorio.DeletAsync(id);
        }

        // ==================== GET ALL ====================
        public async Task<List<AdjudicacionDTO>> GetAllsync()
        {
            return _mapper.Map<List<AdjudicacionDTO>>(await _adjuRepositorio.GetAllasync());
        }

        // ==================== GET BY ID ====================
        public async Task<AdjudicacionDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<AdjudicacionDTO>(await _adjuRepositorio.GetAsync(id));
        }

        // ==================== UPDATE ====================
        public async Task UpdateAsync(UpdateAdjudicacionDTO adjudicacion)
        {
            // ===== REGLA DE NEGOCIO 2 (también al actualizar) =====
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

        // ===== REGLA DE NEGOCIO 4 (método público existente) =====
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