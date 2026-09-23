using Aplicacion.DTO;
using Infraestructura.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteControlador : ControllerBase
    {
        private readonly ReporteRepositorio _reporteRepositorio;
        public ReporteControlador(ReporteRepositorio reporteRepositorio) => _reporteRepositorio = reporteRepositorio;

        [HttpGet("historial-articulo/{idArticulo}")]
        public async Task<IActionResult> HistorialPorArticulo(int idArticulo) =>
            Ok(await _reporteRepositorio.HistorialPorArticuloAsync(idArticulo));

        [HttpGet("ranking-proveedores")]
        public async Task<IActionResult> RankingProveedores([FromQuery] DateTime desde, [FromQuery] DateTime hasta) =>
            Ok(await _reporteRepositorio.RankingProveedoresAsync(desde, hasta));

        [HttpGet("ofertas-orden/{idOrdenCompra}")]
        public async Task<IActionResult> OfertasPorOrden(int idOrdenCompra) =>
            Ok(await _reporteRepositorio.OfertasPorOrdenAsync(idOrdenCompra));

        [HttpGet("pedidos-pendientes")]
        public async Task<IActionResult> PedidosPendientes() =>
            Ok(await _reporteRepositorio.PedidosPendientesAsync());

        [HttpGet("ordenes-activas")]
        public async Task<IActionResult> OrdenesActivas() =>
            Ok(await _reporteRepositorio.OrdenesActivasAsync());

        [HttpGet("gasto-departamental/{idSucursal}/{anio}")]
        public async Task<IActionResult> GastoDepartamental(int idSucursal, int anio) =>
            Ok(await _reporteRepositorio.GastoPorDepartamentoAsync(idSucursal, anio));

        [HttpGet("eficiencia-proceso")]
        public async Task<IActionResult> EficienciaProceso() =>
            Ok(await _reporteRepositorio.EficienciaProcesoAsync());

        [HttpGet("variacion-precios/{idArticulo}/{idProveedor}")]
        public async Task<IActionResult> VariacionPrecios(int idArticulo, int idProveedor) =>
            Ok(await _reporteRepositorio.VariacionPreciosAsync(idArticulo, idProveedor));
    }
}