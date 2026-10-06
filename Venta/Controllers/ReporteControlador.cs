using Aplicacion.DTO;
using Aplicacion.Servicios;
using Infraestructura.Repositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteControlador : ControllerBase
    {
        private readonly ReporteRepositorio _reporteRepositorio;
        private readonly ReportePdfService _reportePdfService;

        public ReporteControlador(
            ReporteRepositorio reporteRepositorio,
            ReportePdfService reportePdfService)
        {
            _reporteRepositorio = reporteRepositorio;
            _reportePdfService = reportePdfService;
        }

        // ============================================================
        // ENDPOINTS EXISTENTES (JSON)
        // ============================================================

        [HttpGet("historial-articulo/{idArticulo}")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> HistorialPorArticulo(int idArticulo) =>
            Ok(await _reporteRepositorio.HistorialPorArticuloAsync(idArticulo));

        [HttpGet("ranking-proveedores")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> RankingProveedores([FromQuery] DateTime desde, [FromQuery] DateTime hasta) =>
            Ok(await _reporteRepositorio.RankingProveedoresAsync(desde, hasta));

        [HttpGet("ofertas-orden/{idOrdenCompra}")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> OfertasPorOrden(int idOrdenCompra) =>
            Ok(await _reporteRepositorio.OfertasPorOrdenAsync(idOrdenCompra));

        [HttpGet("pedidos-pendientes")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> PedidosPendientes() =>
            Ok(await _reporteRepositorio.PedidosPendientesAsync());

        [HttpGet("ordenes-activas")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> OrdenesActivas() =>
            Ok(await _reporteRepositorio.OrdenesActivasAsync());

        [HttpGet("gasto-departamental/{idSucursal}/{anio}")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> GastoDepartamental(int idSucursal, int anio) =>
            Ok(await _reporteRepositorio.GastoPorDepartamentoAsync(idSucursal, anio));

        [HttpGet("eficiencia-proceso")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> EficienciaProceso() =>
            Ok(await _reporteRepositorio.EficienciaProcesoAsync());

        [HttpGet("variacion-precios/{idArticulo}/{idProveedor}")]
        [Authorize(Roles = "Administrador,Auditor,GestorCompras")]
        public async Task<IActionResult> VariacionPrecios(int idArticulo, int idProveedor) =>
            Ok(await _reporteRepositorio.VariacionPreciosAsync(idArticulo, idProveedor));

        // ============================================================
        // NUEVOS ENDPOINTS: PDFs con QuestPDF
        // ============================================================

        [HttpGet("pdf/ranking-proveedores")]
        [AllowAnonymous]
        public IActionResult PdfRankingProveedores()
        {
            try
            {
                var pdf = _reportePdfService.GenerarRankingProveedores();
                // ✅ Sin nombre de archivo → se sirve INLINE (no descarga)
                return File(pdf, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/gasto-departamental")]
        [AllowAnonymous]
        public IActionResult PdfGastoDepartamental()
        {
            try
            {
                var pdf = _reportePdfService.GenerarGastoDepartamental();
                return File(pdf, "application/pdf", $"gasto-departamental-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/ordenes-activas")]
        [AllowAnonymous]
        public IActionResult PdfOrdenesActivas()
        {
            try
            {
                var pdf = _reportePdfService.GenerarOrdenesActivas();
                return File(pdf, "application/pdf", $"ordenes-activas-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/pedidos-pendientes")]
        [AllowAnonymous]
        public IActionResult PdfPedidosPendientes()
        {
            try
            {
                var pdf = _reportePdfService.GenerarPedidosPendientes();
                return File(pdf, "application/pdf", $"pedidos-pendientes-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/historial-articulo")]
        [AllowAnonymous]
        public IActionResult PdfHistorialArticulo()
        {
            try
            {
                var pdf = _reportePdfService.GenerarHistorialArticulo();
                return File(pdf, "application/pdf", $"historial-articulo-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/ofertas-orden")]
        [AllowAnonymous]
        public IActionResult PdfOfertasOrden()
        {
            try
            {
                var pdf = _reportePdfService.GenerarOfertasOrden();
                return File(pdf, "application/pdf", $"ofertas-orden-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/eficiencia-proceso")]
        [AllowAnonymous]
        public IActionResult PdfEficienciaProceso()
        {
            try
            {
                var pdf = _reportePdfService.GenerarEficienciaProceso();
                return File(pdf, "application/pdf", $"eficiencia-proceso-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }

        [HttpGet("pdf/variacion-precios")]
        [AllowAnonymous]
        public IActionResult PdfVariacionPrecios()
        {
            try
            {
                var pdf = _reportePdfService.GenerarVariacionPrecios();
                return File(pdf, "application/pdf", $"variacion-precios-{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message, tipo = ex.GetType().Name });
            }
        }
    }
}