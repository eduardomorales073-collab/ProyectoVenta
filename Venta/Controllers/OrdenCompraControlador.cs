using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdenCompraControlador : ControllerBase
    {
        private readonly IOrdComService _ordComService;
        public OrdenCompraControlador(IOrdComService ordComService) => _ordComService = ordComService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _ordComService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _ordComService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrdenCompraDTO dto)
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var idDepartamento = int.Parse(User.FindFirst("IdDepartamento")?.Value ?? "0");

            await _ordComService.AddAsync(dto, idUsuario, idDepartamento);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateOrdenCompraDTO dto)
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            await _ordComService.UpdateAsync(dto, idUsuario, rol);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ordComService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("con-contadores")]
        public async Task<IActionResult> GetConContadores()
        {
            var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var idDepartamentoStr = User.FindFirst("IdDepartamento")?.Value;
            int? idDepartamento = string.IsNullOrEmpty(idDepartamentoStr)
                ? (int?)null
                : int.Parse(idDepartamentoStr);

            return Ok(await _ordComService.GetConContadoresAsync(idUsuario, rol, idDepartamento));
        }

        [HttpGet("{id}/pedidos")]
        public async Task<IActionResult> GetPedidosDeOrden(int id)
            => Ok(await _ordComService.GetPedidosDeOrdenAsync(id));

        // ==================== ESTADOS ====================

        [HttpPut("{id}/aprobar")]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Aprobar(int id)
        {
            try
            {
                await _ordComService.AprobarAsync(id);
                return Ok(new { mensaje = "Orden aprobada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}/publicar")]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Publicar(int id)
        {
            try
            {
                await _ordComService.PublicarAsync(id);
                return Ok(new { mensaje = "Orden publicada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}/cerrar")]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Cerrar(int id)
        {
            try
            {
                await _ordComService.CerrarAsync(id);
                return Ok(new { mensaje = "Orden cerrada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}/cancelar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                await _ordComService.CancelarAsync(id);
                return Ok(new { mensaje = "Orden cancelada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}