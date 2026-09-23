// TelefonoControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelefonoControlador : ControllerBase
    {
        private readonly ITelefonoService _telefonoService;
        public TelefonoControlador(ITelefonoService telefonoService) => _telefonoService = telefonoService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _telefonoService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _telefonoService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateTelefonoDTO dto)
        {
            await _telefonoService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateTelefonoDTO dto)
        {
            await _telefonoService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _telefonoService.DeleteAsync(id);
            return Ok();
        }
    }
}