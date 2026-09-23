// ProveedorControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorControlador : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        public ProveedorControlador(IProveedorService proveedorService) => _proveedorService = proveedorService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _proveedorService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _proveedorService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateProveedorDTO dto)
        {
            await _proveedorService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProveedorDTO dto)
        {
            await _proveedorService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _proveedorService.DeleteAsync(id);
            return Ok();
        }
    }
}