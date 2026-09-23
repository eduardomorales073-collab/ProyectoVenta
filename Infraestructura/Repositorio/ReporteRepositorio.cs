using Aplicacion.DTO;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorio
{
    public class ReporteRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public ReporteRepositorio(AplicacionDBContexto context) => _context = context;

        // 1. Historial de compras por artículo
        public async Task<List<HistorialCompraDTO>> HistorialPorArticuloAsync(int idArticulo)
        {
            var query =
                from dp in _context.Detalle_Pedido
                join da in _context.Detalle_Adjudicacion on dp.id_Pedido equals da.id_Pedido
                join adj in _context.Adjudicacion on da.id_adjudicacion equals adj.id
                join prov in _context.Proveedor on da.id_Proveedor equals prov.id
                where dp.id_Articulo == idArticulo
                orderby adj.Fecha_Resolucion
                select new HistorialCompraDTO(adj.Fecha_Resolucion, prov.Nombre, da.Precio);

            return await query.ToListAsync();
        }

        // 2. Ranking de proveedores (top 5 por rango de fechas)
        public async Task<List<RankingProveedorDTO>> RankingProveedoresAsync(DateTime desde, DateTime hasta)
        {
            var query =
                from da in _context.Detalle_Adjudicacion
                join adj in _context.Adjudicacion on da.id_adjudicacion equals adj.id
                where adj.Fecha_Resolucion >= desde && adj.Fecha_Resolucion <= hasta
                group da by da.id_Proveedor into g
                select new
                {
                    IdProveedor = g.Key,
                    Total = g.Sum(x => x.Precio * x.Cantidad)
                };

            var top = await query.OrderByDescending(x => x.Total).Take(5).ToListAsync();

            var proveedores = await _context.Proveedor
                .Where(p => top.Select(t => t.IdProveedor).Contains(p.id))
                .ToDictionaryAsync(p => p.id, p => p.Nombre);

            return top.Select(t => new RankingProveedorDTO(t.IdProveedor, proveedores[t.IdProveedor], t.Total)).ToList();
        }

        // 3. Análisis de ofertas por orden de compra
        // ASUME: Pedido_Interno.id_OrdenCompra existe
        public async Task<List<OfertaComparativaDTO>> OfertasPorOrdenAsync(int idOrdenCompra)
        {
            var ofertas =
                from p in _context.Pedido_Interno
                where p.id_OrdenCompra == idOrdenCompra
                join of in _context.Oferta_Proveedor on p.id equals of.id_Pedido_Interno
                join prov in _context.Proveedor on of.id_Proveedor equals prov.id
                select new { of.id_Proveedor, prov.Nombre, of.Precio, of.Fecha_Oferta };

            var lista = await ofertas.ToListAsync();
            if (!lista.Any()) return new List<OfertaComparativaDTO>();

            var precioMinimo = lista.Min(x => x.Precio);

            return lista.Select(x => new OfertaComparativaDTO(
                x.id_Proveedor, x.Nombre, x.Precio, x.Fecha_Oferta, x.Precio == precioMinimo)).ToList();
        }

        // 4. Pedidos internos sin asignar a una orden de compra
        // ASUME: Pedido_Interno.id_OrdenCompra es nullable (int?)
        public async Task<List<PedidoPendienteDTO>> PedidosPendientesAsync()
        {
            var query = _context.Pedido_Interno
                .Where(p => p.id_OrdenCompra == null)
                .Select(p => new PedidoPendienteDTO(p.id, p.id_Departamento, p.Fecha_Solicitada));

            return await query.ToListAsync();
        }

        // 5. Órdenes activas (lee la VIEW creada en SQL)
        public async Task<List<OrdenActivaDTO>> OrdenesActivasAsync()
        {
            return await _context.Database
                .SqlQuery<OrdenActivaDTO>($"SELECT Id, Descripcion, Fecha_Creacion, Fecha_Limite FROM Vista_OrdenesActivas")
                .ToListAsync();
        }

        // 6. Gasto por departamento en una sucursal, para un año dado
        public async Task<List<GastoDepartamentalDTO>> GastoPorDepartamentoAsync(int idSucursal, int anio)
        {
            var query =
                from dep in _context.Departamento
                where dep.id_Sucursal == idSucursal
                join p in _context.Pedido_Interno on dep.id equals p.id_Departamento into pedidos
                from p in pedidos
                join da in _context.Detalle_Adjudicacion on p.id equals da.id_Pedido into detalles
                from da in detalles
                join adj in _context.Adjudicacion on da.id_adjudicacion equals adj.id
                where adj.Fecha_Resolucion.Year == anio
                group new { da, dep } by new { dep.id, dep.Nombre } into g
                select new GastoDepartamentalDTO(
                    g.Key.id,
                    g.Key.Nombre,
                    g.Sum(x => x.da.Precio * x.da.Cantidad));

            return await query.ToListAsync();
        }

        // 7. Eficiencia del proceso de compra (promedio de días)
        public async Task<EficienciaCompraDTO> EficienciaProcesoAsync()
        {
            var dias =
                from adj in _context.Adjudicacion
                join oc in _context.Orden_Compra on adj.Orden_Compra equals oc.id
                select EF.Functions.DateDiffDay(oc.Fecha_Creacion, adj.Fecha_Resolucion);

            var lista = await dias.ToListAsync();
            var promedio = lista.Any() ? lista.Average() : 0;

            return new EficienciaCompraDTO(promedio);
        }

        // 8. Variación de precios para un artículo y proveedor
        public async Task<List<VariacionPrecioDTO>> VariacionPreciosAsync(int idArticulo, int idProveedor)
        {
            var query =
                from dp in _context.Detalle_Pedido
                where dp.id_Articulo == idArticulo
                join da in _context.Detalle_Adjudicacion on dp.id_Pedido equals da.id_Pedido
                where da.id_Proveedor == idProveedor
                join adj in _context.Adjudicacion on da.id_adjudicacion equals adj.id
                orderby adj.Fecha_Resolucion
                select new VariacionPrecioDTO(adj.Fecha_Resolucion, da.Precio);

            return await query.ToListAsync();
        }
    }
}