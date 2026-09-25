using System.Security.Claims;
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // ← Cualquier usuario autenticado (sin rol específico)
    public class PerfilControlador : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilControlador(IUsuarioService usuarioService)
            => _usuarioService = usuarioService;

        private int GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(claim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetPerfil()
        {
            var perfil = await _usuarioService.ObtenerPerfilAsync(GetUserId());
            return perfil == null ? NotFound() : Ok(perfil);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePerfil([FromBody] UpdatePerfilDTO dto)
        {
            var (ok, mensaje) = await _usuarioService.ActualizarPerfilAsync(GetUserId(), dto);
            return ok ? Ok(new { mensaje }) : BadRequest(new { mensaje });
        }

        [HttpPost("cambiar-contrasena")]
        public async Task<IActionResult> CambiarContrasena([FromBody] ChangePasswordDTO dto)
        {
            var (ok, mensaje) = await _usuarioService.CambiarContrasenaAsync(GetUserId(), dto);
            return ok ? Ok(new { mensaje }) : BadRequest(new { mensaje });
        }
    }
}