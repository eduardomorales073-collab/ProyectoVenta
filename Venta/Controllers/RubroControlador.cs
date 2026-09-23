// RubroControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RubroControlador : ControllerBase
    {
        private readonly IRubroService _rubroService;
        public RubroControlador(IRubroService rubroService) => _rubroService = rubroService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _rubroService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _rubroService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateRubroDTO dto)
        {
            await _rubroService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateRubroDTO dto)
        {
            await _rubroService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rubroService.DeleteAsync(id);
            return Ok();
        }
    }
}