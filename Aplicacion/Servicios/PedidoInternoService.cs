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
        private readonly OferProvRepositorio _ofertaRepositorio;
        private readonly DetaAdjRepositorio _detalleAdjRepositorio;
        private readonly ProveedorRepositorio _proveedorRepositorio;
        private readonly DetaPedRepositorio _detallePedidoRepositorio;
        private readonly ArticuRepositorio _articuloRepositorio;
        private readonly ProveArtRepositorio _proveArtRepositorio;
        private readonly ProveRubRepositorio _proveRubRepositorio;
        private readonly RelacionRepositorio _relacionRepositorio;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public PedidoInternoService(
            PedIntRepositorio pedidoInternoRepository,
            OrdComRepositorio ordenCompraRepositorio,
            DepartaRepositorio departamentoRepositorio,
            SucursalRepositorio sucursalRepositorio,
            OferProvRepositorio ofertaRepositorio,
            DetaAdjRepositorio detalleAdjRepositorio,
            ProveedorRepositorio proveedorRepositorio,
            DetaPedRepositorio detallePedidoRepositorio,
            ArticuRepositorio articuloRepositorio,
            ProveArtRepositorio proveArtRepositorio,
            ProveRubRepositorio proveRubRepositorio,
            RelacionRepositorio relacionRepositorio,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _mapper = mapper;
            _pedidoInternoRepositorio = pedidoInternoRepository;
            _ordenCompraRepositorio = ordenCompraRepositorio;
            _departamentoRepositorio = departamentoRepositorio;
            _sucursalRepositorio = sucursalRepositorio;
            _ofertaRepositorio = ofertaRepositorio;
            _detalleAdjRepositorio = detalleAdjRepositorio;
            _proveedorRepositorio = proveedorRepositorio;
            _detallePedidoRepositorio = detallePedidoRepositorio;
            _articuloRepositorio = articuloRepositorio;
            _proveArtRepositorio = proveArtRepositorio;
            _proveRubRepositorio = proveRubRepositorio;
            _relacionRepositorio = relacionRepositorio;
            _publishEndpoint = publishEndpoint;
        }

        // ==================== ADD (CREATE) ====================
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

                if (pedido.Fecha_Solicitada < orden.Fecha_Creacion)
                {
                    throw new InvalidOperationException(
                        $"La fecha límite de ofertas ({pedido.Fecha_Solicitada:dd/MM/yyyy}) " +
                        $"no puede ser anterior a la fecha de creación de la orden " +
                        $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                    );
                }
            }

            var todos = await _pedidoInternoRepositorio.GetAllasync();
            var nuevoPedido = _mapper.Map<Pedido_Interno>(pedido);
            nuevoPedido.id = todos.Any() ? todos.Max(p => p.id) + 1 : 1;
            nuevoPedido.Observaciones = pedido.Observaciones;

            if (string.IsNullOrEmpty(nuevoPedido.codigo))
            {
                var anio = DateTime.Now.Year;
                nuevoPedido.codigo = $"PED-{anio}-{nuevoPedido.id.ToString("D3")}";
            }

            if (!nuevoPedido.cantidad.HasValue)
            {
                nuevoPedido.cantidad = 1;
            }

            nuevoPedido.Fecha_Ingreso = DateTime.Now;

            await _pedidoInternoRepositorio.AddAsync(nuevoPedido);

            // ✅ GUARDAR LOS ARTÍCULOS DEL PEDIDO
            if (pedido.Articulos != null && pedido.Articulos.Any())
            {
                foreach (var art in pedido.Articulos)
                {
                    var detalle = new Detalle_Pedido
                    {
                        id_Pedido = nuevoPedido.id,
                        id_Articulo = art.id_Articulo,
                        Cantidad = art.Cantidad
                    };
                    await _detallePedidoRepositorio.AddAsync(detalle);
                }
            }

            // ✅ PUBLICAR AUTOMÁTICAMENTE LA ORDEN (Aprobada → Publicada)
            if (pedido.id_OrdenCompra.HasValue)
            {
                var ordenActualizar = await _ordenCompraRepositorio.GetAsync(pedido.id_OrdenCompra.Value);
                if (ordenActualizar != null && ordenActualizar.Estado == "Aprobada")
                {
                    ordenActualizar.Estado = "Publicada";
                    await _ordenCompraRepositorio.UpdateAsync(ordenActualizar);
                }
            }

            await _publishEndpoint.Publish(new PedidoCreado(
                PedidoId: nuevoPedido.id,
                Codigo: nuevoPedido.codigo ?? "",
                IdDepartamento: nuevoPedido.id_Departamento,
                Cantidad: nuevoPedido.cantidad ?? 1,
                Fecha: DateTime.UtcNow,
                Usuario: "Sistema"
            ));
        }

        // ==================== DELETE ====================
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

        // ==================== GET ALL ====================
        public async Task<List<PedidoInternoDTO>> GetAllsync()
        {
            var pedidos = await _pedidoInternoRepositorio.GetAllasync();
            var departamentos = await _departamentoRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();
            var ofertas = await _ofertaRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();
            var detallesPedido = await _detallePedidoRepositorio.GetAllasync();
            var articulos = await _articuloRepositorio.GetAllasync();

            return pedidos
                .OrderByDescending(p => p.urgente)
                .ThenByDescending(p => p.id)
                .Select(p =>
                {
                    var depto = departamentos.FirstOrDefault(d => d.id == p.id_Departamento);
                    var sucursal = depto != null
                        ? sucursales.FirstOrDefault(s => s.id == depto.id_Sucursal)
                        : null;

                    var ofertasDelPedido = ofertas.Where(o => o.id_Pedido_Interno == p.id).ToList();
                    var totalOfertas = ofertasDelPedido.Count;

                    var detalleAdj = detallesAdj.FirstOrDefault(d => d.id_Pedido == p.id);
                    var adjudicado = detalleAdj != null;

                    int? idProveedorGanador = null;
                    string? nombreProveedorGanador = null;
                    decimal? precioAdjudicado = null;

                    if (adjudicado && detalleAdj != null)
                    {
                        idProveedorGanador = detalleAdj.id_Proveedor;
                        precioAdjudicado = detalleAdj.Precio;
                        var proveedor = proveedores.FirstOrDefault(pr => pr.id == detalleAdj.id_Proveedor);
                        nombreProveedorGanador = proveedor?.Nombre;
                    }

                    var articulosDelPedido = detallesPedido
                        .Where(dp => dp.id_Pedido == p.id)
                        .Select(dp =>
                        {
                            var art = articulos.FirstOrDefault(a => a.id == dp.id_Articulo);
                            return new ArticuloDePedidoDTO(
                                dp.id_Articulo,
                                art?.codigo ?? "",
                                art?.Nombre ?? $"Artículo #{dp.id_Articulo}",
                                dp.Cantidad,
                                null
                            );
                        })
                        .ToList();

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
                        sucursal?.id,
                        p.urgente,
                        totalOfertas,
                        adjudicado,
                        idProveedorGanador,
                        nombreProveedorGanador,
                        precioAdjudicado,
                        p.Observaciones,
                        articulosDelPedido
                    );
                }).ToList();
        }

        // ==================== GET BY ID ====================
        public async Task<PedidoInternoDTO> GetByIdAsync(int id)
        {
            var pedido = await _pedidoInternoRepositorio.GetAsync(id);
            if (pedido == null) return null;

            var departamentos = await _departamentoRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();
            var ofertas = await _ofertaRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();
            var detallesPedido = await _detallePedidoRepositorio.GetAllasync();
            var articulos = await _articuloRepositorio.GetAllasync();

            var depto = departamentos.FirstOrDefault(d => d.id == pedido.id_Departamento);
            var sucursal = depto != null
                ? sucursales.FirstOrDefault(s => s.id == depto.id_Sucursal)
                : null;

            var ofertasDelPedido = ofertas.Where(o => o.id_Pedido_Interno == pedido.id).ToList();
            var totalOfertas = ofertasDelPedido.Count;

            var detalleAdj = detallesAdj.FirstOrDefault(d => d.id_Pedido == pedido.id);
            var adjudicado = detalleAdj != null;

            int? idProveedorGanador = null;
            string? nombreProveedorGanador = null;
            decimal? precioAdjudicado = null;

            if (adjudicado && detalleAdj != null)
            {
                idProveedorGanador = detalleAdj.id_Proveedor;
                precioAdjudicado = detalleAdj.Precio;
                var proveedor = proveedores.FirstOrDefault(pr => pr.id == detalleAdj.id_Proveedor);
                nombreProveedorGanador = proveedor?.Nombre;
            }

            var articulosDelPedido = detallesPedido
                .Where(dp => dp.id_Pedido == pedido.id)
                .Select(dp =>
                {
                    var art = articulos.FirstOrDefault(a => a.id == dp.id_Articulo);
                    return new ArticuloDePedidoDTO(
                        dp.id_Articulo,
                        art?.codigo ?? "",
                        art?.Nombre ?? $"Artículo #{dp.id_Articulo}",
                        dp.Cantidad,
                        null
                    );
                })
                .ToList();

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
                sucursal?.id,
                pedido.urgente,
                totalOfertas,
                adjudicado,
                idProveedorGanador,
                nombreProveedorGanador,
                precioAdjudicado,
                pedido.Observaciones,
                articulosDelPedido
            );
        }

        // ==================== UPDATE ====================
        public async Task UpdateAsync(UpdatePedidoInternoDTO pedido)
        {
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

                if (pedido.Fecha_Solicitada < orden.Fecha_Creacion)
                {
                    throw new InvalidOperationException(
                        $"La fecha límite de ofertas ({pedido.Fecha_Solicitada:dd/MM/yyyy}) " +
                        $"no puede ser anterior a la fecha de creación de la orden " +
                        $"#{orden.id} ({orden.Fecha_Creacion:dd/MM/yyyy})."
                    );
                }
            }

            // ===== ACTUALIZAR LA ENTIDAD YA RASTREADA =====
            pedidoExistente.codigo = pedido.codigo;
            pedidoExistente.cantidad = pedido.cantidad ?? 1;
            pedidoExistente.id_Departamento = pedido.id_Departamento;
            pedidoExistente.id_OrdenCompra = pedido.id_OrdenCompra;
            pedidoExistente.Fecha_Solicitada = pedido.Fecha_Solicitada;
            pedidoExistente.urgente = pedido.urgente;
            pedidoExistente.Observaciones = pedido.Observaciones;

            await _pedidoInternoRepositorio.UpdateAsync(pedidoExistente);

            // ===== ACTUALIZAR LOS ARTÍCULOS (BORRAR + RECREAR) =====
            if (pedido.Articulos != null)
            {
                var detallesExistentes = await _detallePedidoRepositorio.GetAllasync();
                var detallesDelPedido = detallesExistentes.Where(d => d.id_Pedido == pedido.id).ToList();

                foreach (var det in detallesDelPedido)
                {
                    await _detallePedidoRepositorio.DeletAsync(det.id_Pedido, det.id_Articulo);
                }

                foreach (var art in pedido.Articulos)
                {
                    var nuevoDetalle = new Detalle_Pedido
                    {
                        id_Pedido = pedido.id,
                        id_Articulo = art.id_Articulo,
                        Cantidad = art.Cantidad
                    };
                    await _detallePedidoRepositorio.AddAsync(nuevoDetalle);
                }
            }

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

        // ==================== GET BY DEPARTAMENTO ====================
        public async Task<List<PedidoInternoDTO>> GetByDepartamentoAsync(int idDepartamento)
        {
            var todos = await GetAllsync();
            return todos.Where(p => p.id_Departamento == idDepartamento).ToList();
        }

        // ==================== GET DISPONIBLES PARA PROVEEDOR ====================
        public async Task<List<PedidoDisponibleProveedorDTO>> GetDisponiblesParaProveedorAsync(int idProveedor)
        {
            // 1. Obtener los artículos que provee este proveedor
            var proveedorArticulos = await _proveArtRepositorio.GetAllasync();
            var misArticulos = proveedorArticulos
                .Where(pa => pa.id_Proveedor == idProveedor)
                .Select(pa => pa.id_Articulo)
                .ToHashSet();

            if (!misArticulos.Any())
                return new List<PedidoDisponibleProveedorDTO>();

            // 2. Obtener los rubros del proveedor
            var proveedorRubros = await _proveRubRepositorio.GetAllasync();
            var misRubros = proveedorRubros
                .Where(pr => pr.id_Proveedor == idProveedor)
                .Select(pr => pr.id_Rubro)
                .ToHashSet();

            // 3. Obtener TODOS los datos necesarios
            var pedidos = await _pedidoInternoRepositorio.GetAllasync();
            var detallesPedido = await _detallePedidoRepositorio.GetAllasync();
            var ofertas = await _ofertaRepositorio.GetAllasync();
            var articulos = await _articuloRepositorio.GetAllasync();
            var departamentos = await _departamentoRepositorio.GetAllasync();
            var sucursales = await _sucursalRepositorio.GetAllasync();
            var detallesAdj = await _detalleAdjRepositorio.GetAllasync();
            var relaciones = await _relacionRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();

            // ✅ Obtener IDs de proveedores relacionados (relación bidireccional)
            var idsRelacionados = relaciones
                .Where(r => r.Proveedor1 == idProveedor || r.Proveedor2 == idProveedor)
                .Select(r => r.Proveedor1 == idProveedor ? r.Proveedor2 : r.Proveedor1)
                .ToHashSet();

            // 4. Filtrar pedidos con artículos que el proveedor maneja
            var pedidosFiltrados = pedidos
                .Where(p =>
                {
                    var articulosDelPedidoIds = detallesPedido
                        .Where(dp => dp.id_Pedido == p.id)
                        .Select(dp => dp.id_Articulo)
                        .ToList();

                    return articulosDelPedidoIds.Any(a => misArticulos.Contains(a));
                })
                .ToList();

            // 5. Para cada pedido, calcular competencia y construir el DTO
            return pedidosFiltrados.Select(p =>
            {
                var ofertasDelPedido = ofertas.Where(o => o.id_Pedido_Interno == p.id).ToList();
                var miOferta = ofertasDelPedido.FirstOrDefault(o => o.id_Proveedor == idProveedor);

                var idsProveedoresDelMismoRubro = proveedorRubros
                    .Where(pr => misRubros.Contains(pr.id_Rubro) && pr.id_Proveedor != idProveedor)
                    .Select(pr => pr.id_Proveedor)
                    .ToHashSet();

                var ofertasDelRubro = ofertasDelPedido
                    .Where(o => idsProveedoresDelMismoRubro.Contains(o.id_Proveedor))
                    .ToList();

                decimal? precioMin = ofertasDelRubro.Any()
                    ? ofertasDelRubro.Min(o => o.Precio)
                    : null;

                // ✅ Proveedores relacionados que ofertaron
                var ofertasRelacionados = ofertasDelPedido
                    .Where(o => idsRelacionados.Contains(o.id_Proveedor))
                    .ToList();

                var proveedoresRelacionados = ofertasRelacionados.Select(o =>
                {
                    var prov = proveedores.FirstOrDefault(pr => pr.id == o.id_Proveedor);
                    var relacion = relaciones.FirstOrDefault(r =>
                        (r.Proveedor1 == idProveedor && r.Proveedor2 == o.id_Proveedor) ||
                        (r.Proveedor2 == idProveedor && r.Proveedor1 == o.id_Proveedor)
                    );

                    return new ProveedorRelacionadoDTO(
                        o.id_Proveedor,
                        prov?.Nombre ?? $"Proveedor #{o.id_Proveedor}",
                        relacion?.TipoRelacion ?? "Relacionado",
                        o.Precio
                    );
                }).ToList();

                // Artículos del pedido
                var articulosDelPedido = detallesPedido
                    .Where(dp => dp.id_Pedido == p.id)
                    .Select(dp =>
                    {
                        var art = articulos.FirstOrDefault(a => a.id == dp.id_Articulo);
                        return new ArticuloDePedidoDTO(
                            dp.id_Articulo,
                            art?.codigo ?? "",
                            art?.Nombre ?? $"Artículo #{dp.id_Articulo}",
                            dp.Cantidad,
                            art?.unidad_medida
                        );
                    })
                    .ToList();

                var depto = departamentos.FirstOrDefault(d => d.id == p.id_Departamento);
                var sucursal = depto != null
                    ? sucursales.FirstOrDefault(s => s.id == depto.id_Sucursal)
                    : null;
                var estaAdjudicado = detallesAdj.Any(d => d.id_Pedido == p.id);

                return new PedidoDisponibleProveedorDTO(
                    p.id,
                    p.codigo,
                    p.cantidad,
                    p.id_Departamento,
                    depto?.Nombre,
                    p.id_OrdenCompra,
                    p.Fecha_Solicitada,
                    p.Fecha_Ingreso,
                    sucursal?.Nombre,
                    sucursal?.id,
                    p.urgente,
                    p.Observaciones,
                    ofertasDelPedido.Count,
                    estaAdjudicado,
                    articulosDelPedido,
                    ofertasDelRubro.Count,
                    precioMin,
                    miOferta != null,
                    miOferta?.Precio,
                    proveedoresRelacionados
                );
            }).ToList();
        }
    }
}