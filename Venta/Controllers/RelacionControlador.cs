// RelacionControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelacionControlador : ControllerBase
    {
        private readonly IRelacionService _relacionService;
        public RelacionControlador(IRelacionService relacionService) => _relacionService = relacionService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _relacionService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _relacionService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateRelacionDTO dto)
        {
            await _relacionService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateRelacionDTO dto)
        {
            await _relacionService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _relacionService.DeleteAsync(id);
            return Ok();
        }
    }
}