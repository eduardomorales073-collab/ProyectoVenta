// DireccionControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionControlador : ControllerBase
    {
        private readonly IDireccionService _direccionService;
        public DireccionControlador(IDireccionService direccionService) => _direccionService = direccionService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _direccionService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _direccionService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateDireccionDTO dto)
        {
            await _direccionService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDireccionDTO dto)
        {
            await _direccionService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _direccionService.DeleteAsync(id);
            return Ok();
        }
    }
}