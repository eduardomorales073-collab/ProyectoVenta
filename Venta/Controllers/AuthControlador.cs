using Aplicacion.DTO;
using Aplicacion.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthControlador : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthControlador(AuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var resultado = await _authService.LoginAsync(dto);
            if (resultado == null) return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            return Ok(resultado);
        }
    }
}