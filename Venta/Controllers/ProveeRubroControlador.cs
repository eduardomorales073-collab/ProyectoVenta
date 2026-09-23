// ProveeRubroControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveeRubroControlador : ControllerBase
    {
        private readonly IProvRubService _provRubService;
        public ProveeRubroControlador(IProvRubService provRubService) => _provRubService = provRubService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _provRubService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _provRubService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateProveeRubroDTO dto)
        {
            await _provRubService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProveeRubroDTO dto)
        {
            await _provRubService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _provRubService.DeleteAsync(id);
            return Ok();
        }
    }
}