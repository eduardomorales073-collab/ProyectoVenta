// PermisosControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]  // ← NUEVO
    public class PermisosControlador : ControllerBase
    {
        private readonly IPermisosService _permisosService;
        public PermisosControlador(IPermisosService permisosService) => _permisosService = permisosService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _permisosService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _permisosService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreatePermisosDTO dto)
        {
            await _permisosService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePermisosDTO dto)
        {
            await _permisosService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _permisosService.DeleteAsync(id);
            return Ok();
        }
    }
}