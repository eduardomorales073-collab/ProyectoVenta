using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalControlador : ControllerBase
    {
        private readonly ISucursalService _sucursalService;
        public SucursalControlador(ISucursalService sucursalService) => _sucursalService = sucursalService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _sucursalService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _sucursalService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateSucursalDTO dto)
        {
            await _sucursalService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateSucursalDTO dto)
        {
            await _sucursalService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sucursalService.DeleteAsync(id);
            return Ok();
        }
    }
}