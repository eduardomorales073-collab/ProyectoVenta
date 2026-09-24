// UsuarioControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Aplicacion.Repositorio;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class UsuarioControlador : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioControlador(IUsuarioService usuarioService) => _usuarioService = usuarioService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _usuarioService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _usuarioService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateUsuariosDTO dto)
        {
            await _usuarioService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUsuariosDTO dto)
        {
            await _usuarioService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _usuarioService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("{id}/permisos")]
        public async Task<IActionResult> GetPermisos(int id)
        {
            var permisos = await _usuarioService.ObtenerPermisosAsync(id);
            if (permisos == null) return NotFound();
            return Ok(permisos);
        }

        // ← NUEVO ENDPOINT
        [HttpPut("{id}/permisos")]
        public async Task<IActionResult> UpdatePermisos(int id, UpdatePermisosDTO dto)
        {
            var ok = await _usuarioService.ActualizarPermisosAsync(id, dto);
            if (!ok) return NotFound();
            return Ok();
        }
    }
}