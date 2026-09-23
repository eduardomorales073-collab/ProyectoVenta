// RolesControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesControlador : ControllerBase
    {
        private readonly IRolesService _rolesService;
        public RolesControlador(IRolesService rolesService) => _rolesService = rolesService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _rolesService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _rolesService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateRolesDTO dto)
        {
            await _rolesService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateRolesDTO dto)
        {
            await _rolesService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rolesService.DeleteAsync(id);
            return Ok();
        }
    }
}