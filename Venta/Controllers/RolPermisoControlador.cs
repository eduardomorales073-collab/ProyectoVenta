// RolPermisoControlador.cs (2 claves)
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolPermisoControlador : ControllerBase
    {
        private readonly IRolPermiService _rolPermiService;
        public RolPermisoControlador(IRolPermiService rolPermiService) => _rolPermiService = rolPermiService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _rolPermiService.GetAllsync());

        [HttpGet("{idRol}/{idPermiso}")]
        public async Task<IActionResult> GetById(int idRol, int idPermiso) =>
            Ok(await _rolPermiService.GetByIdAsync(idRol, idPermiso));

        [HttpPost]
        public async Task<IActionResult> Create(CreateRolPermisoDTO dto)
        {
            await _rolPermiService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateRolPermisoDTO dto)
        {
            await _rolPermiService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{idRol}/{idPermiso}")]
        public async Task<IActionResult> Delete(int idRol, int idPermiso)
        {
            await _rolPermiService.DeleteAsync(idRol, idPermiso);
            return Ok();
        }
    }
}