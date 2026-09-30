using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.Modelos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IMongoCollection<CompraHistorial> _compras;
        private readonly IMongoCollection<PedidoHistorial> _pedidos;
        private readonly IMongoCollection<OfertaHistorial> _ofertas;
        private readonly IMongoCollection<OrdenHistorial> _ordenes;
        private readonly IMongoCollection<CancelacionHistorial> _cancelaciones;
        private readonly IMongoCollection<AdjudicacionHistorial> _adjudicaciones;
        private readonly IMongoCollection<PedidoActualizacionHistorial> _pedidosActualizados;
        private readonly IMongoCollection<OfertaActualizacionHistorial> _ofertasActualizadas;

        public AuditoriaService(
            IMongoCollection<CompraHistorial> compras,
            IMongoCollection<PedidoHistorial> pedidos,
            IMongoCollection<OfertaHistorial> ofertas,
            IMongoCollection<OrdenHistorial> ordenes,
            IMongoCollection<CancelacionHistorial> cancelaciones,
            IMongoCollection<AdjudicacionHistorial> adjudicaciones,
            IMongoCollection<PedidoActualizacionHistorial> pedidosActualizados,
            IMongoCollection<OfertaActualizacionHistorial> ofertasActualizadas)
        {
            _compras = compras;
            _pedidos = pedidos;
            _ofertas = ofertas;
            _ordenes = ordenes;
            _cancelaciones = cancelaciones;
            _adjudicaciones = adjudicaciones;
            _pedidosActualizados = pedidosActualizados;
            _ofertasActualizadas = ofertasActualizadas;
        }

        public async Task<List<EventoAuditoriaDTO>> GetHistorialAsync(
            string? tipo = null,
            DateTime? fechaDesde = null,
            DateTime? fechaHasta = null)
        {
            var eventos = new List<EventoAuditoriaDTO>();

            // ===== 1. COMPRAS =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("Compra", StringComparison.OrdinalIgnoreCase))
            {
                var compras = await _compras.Find(_ => true).ToListAsync();
                eventos.AddRange(compras.Select(c => new EventoAuditoriaDTO(
                    Id: c.Id ?? "",
                    Tipo: "Compra",
                    Fecha: c.Fecha,
                    Titulo: $"Orden #{c.OrdenId}",
                    Detalle: c.Detalle,
                    Usuario: null,
                    Referencia: c.OrdenId.ToString()
                )));
            }

            // ===== 2. PEDIDOS =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("Pedido", StringComparison.OrdinalIgnoreCase))
            {
                var pedidos = await _pedidos.Find(_ => true).ToListAsync();
                eventos.AddRange(pedidos.Select(p => new EventoAuditoriaDTO(
                    Id: p.Id ?? "",
                    Tipo: "Pedido",
                    Fecha: p.Fecha,
                    Titulo: p.Codigo,
                    Detalle: $"Cantidad: {p.Cantidad} | Departamento: #{p.IdDepartamento}",
                    Usuario: p.Usuario,
                    Referencia: p.PedidoId.ToString()
                )));
            }

            // ===== 3. OFERTAS =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("Oferta", StringComparison.OrdinalIgnoreCase))
            {
                var ofertas = await _ofertas.Find(_ => true).ToListAsync();
                eventos.AddRange(ofertas.Select(o => new EventoAuditoriaDTO(
                    Id: o.Id ?? "",
                    Tipo: "Oferta",
                    Fecha: o.Fecha,
                    Titulo: $"Oferta #{o.OfertaId}",
                    Detalle: $"{o.Proveedor} ofertó Q {o.Precio:N2} para Pedido #{o.IdPedidoInterno}",
                    Usuario: null,
                    Referencia: o.OfertaId.ToString()
                )));
            }

            // ===== 4. ÓRDENES =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("Orden", StringComparison.OrdinalIgnoreCase))
            {
                var ordenes = await _ordenes.Find(_ => true).ToListAsync();
                eventos.AddRange(ordenes.Select(o => new EventoAuditoriaDTO(
                    Id: o.Id ?? "",
                    Tipo: "Orden",
                    Fecha: o.Fecha_Creacion,
                    Titulo: $"Orden #{o.OrdenId}",
                    Detalle: $"{o.Descripcion} | Límite: {o.Fecha_Limite:dd/MM/yyyy}",
                    Usuario: null,
                    Referencia: o.OrdenId.ToString()
                )));
            }

            // ===== 5. CANCELACIONES =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("Cancelacion", StringComparison.OrdinalIgnoreCase))
            {
                var cancelaciones = await _cancelaciones.Find(_ => true).ToListAsync();
                eventos.AddRange(cancelaciones.Select(c => new EventoAuditoriaDTO(
                    Id: c.Id ?? "",
                    Tipo: "Cancelacion",
                    Fecha: c.Fecha,
                    Titulo: c.Codigo,
                    Detalle: $"Motivo: {c.Motivo}",
                    Usuario: c.Usuario,
                    Referencia: c.PedidoId.ToString()
                )));
            }

            // ===== 6. ADJUDICACIONES =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("Adjudicacion", StringComparison.OrdinalIgnoreCase))
            {
                var adjudicaciones = await _adjudicaciones.Find(_ => true).ToListAsync();
                eventos.AddRange(adjudicaciones.Select(a => new EventoAuditoriaDTO(
                    Id: a.Id ?? "",
                    Tipo: "Adjudicacion",
                    Fecha: a.Fecha,
                    Titulo: $"Adjudicación #{a.AdjudicacionId}",
                    Detalle: $"Orden #{a.OrdenCompra} | Estado: {a.Estado} | Resolución: {a.Fecha_Resolucion:dd/MM/yyyy}",
                    Usuario: a.Usuario,
                    Referencia: a.AdjudicacionId.ToString()
                )));
            }

            // ===== 7. PEDIDOS ACTUALIZADOS =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("PedidoActualizado", StringComparison.OrdinalIgnoreCase))
            {
                var pedidosAct = await _pedidosActualizados.Find(_ => true).ToListAsync();
                eventos.AddRange(pedidosAct.Select(p => new EventoAuditoriaDTO(
                    Id: p.Id ?? "",
                    Tipo: "PedidoActualizado",
                    Fecha: p.Fecha,
                    Titulo: p.Codigo,
                    Detalle: $"Cantidad actualizada: {p.Cantidad} | Departamento: #{p.IdDepartamento}",
                    Usuario: p.Usuario,
                    Referencia: p.PedidoId.ToString()
                )));
            }

            // ===== 8. OFERTAS ACTUALIZADAS =====
            if (string.IsNullOrEmpty(tipo) || tipo.Equals("OfertaActualizada", StringComparison.OrdinalIgnoreCase))
            {
                var ofertasAct = await _ofertasActualizadas.Find(_ => true).ToListAsync();
                eventos.AddRange(ofertasAct.Select(o => new EventoAuditoriaDTO(
                    Id: o.Id ?? "",
                    Tipo: "OfertaActualizada",
                    Fecha: o.Fecha,
                    Titulo: $"Oferta #{o.OfertaId}",
                    Detalle: $"{o.Proveedor}: Q {o.PrecioAnterior:N2} → Q {o.PrecioNuevo:N2}",
                    Usuario: null,
                    Referencia: o.OfertaId.ToString()
                )));
            }

            // ===== FILTROS DE FECHA =====
            if (fechaDesde.HasValue)
            {
                eventos = eventos.Where(e => e.Fecha >= fechaDesde.Value).ToList();
            }
            if (fechaHasta.HasValue)
            {
                eventos = eventos.Where(e => e.Fecha <= fechaHasta.Value).ToList();
            }

            // ===== ORDENAR POR FECHA DESCENDENTE =====
            return eventos.OrderByDescending(e => e.Fecha).ToList();
        }
    }
}