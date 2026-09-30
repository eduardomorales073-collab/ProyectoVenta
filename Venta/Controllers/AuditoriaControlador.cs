using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditoriaControlador : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaControlador(IAuditoriaService auditoriaService)
            => _auditoriaService = auditoriaService;

        /// <summary>
        /// Historial de auditoría unificado con filtros opcionales.
        /// </summary>
        [HttpGet("historial")]
        [Authorize(Roles = "Administrador,GestorCompras,Auditor")]
        public async Task<IActionResult> GetHistorial(
            [FromQuery] string? tipo = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null)
        {
            var eventos = await _auditoriaService.GetHistorialAsync(tipo, fechaDesde, fechaHasta);
            return Ok(eventos);
        }
    }
}