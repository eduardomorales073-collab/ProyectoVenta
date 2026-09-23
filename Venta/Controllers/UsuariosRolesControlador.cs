// UsuariosRolesControlador.cs (2 claves)
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosRolesControlador : ControllerBase
    {
        private readonly IUsuarioRolesService _usuarioRolesService;
        public UsuariosRolesControlador(IUsuarioRolesService usuarioRolesService) => _usuarioRolesService = usuarioRolesService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _usuarioRolesService.GetAllsync());

        [HttpGet("{idUsuario}/{idRol}")]
        public async Task<IActionResult> GetById(int idUsuario, int idRol) =>
            Ok(await _usuarioRolesService.GetByIdAsync(idUsuario, idRol));

        [HttpPost]
        public async Task<IActionResult> Create(CreateUsuariosRolesDTO dto)
        {
            await _usuarioRolesService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUsuariosRolesDTO dto)
        {
            await _usuarioRolesService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{idUsuario}/{idRol}")]
        public async Task<IActionResult> Delete(int idUsuario, int idRol)
        {
            await _usuarioRolesService.DeleteAsync(idUsuario, idRol);
            return Ok();
        }
    }
}