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

        public AuditoriaService(
            IMongoCollection<CompraHistorial> compras,
            IMongoCollection<PedidoHistorial> pedidos,
            IMongoCollection<OfertaHistorial> ofertas,
            IMongoCollection<OrdenHistorial> ordenes,
            IMongoCollection<CancelacionHistorial> cancelaciones)
        {
            _compras = compras;
            _pedidos = pedidos;
            _ofertas = ofertas;
            _ordenes = ordenes;
            _cancelaciones = cancelaciones;
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