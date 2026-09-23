// AdjudicacionControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdjudicacionControlador : ControllerBase
    {
        private readonly IAdjudicacionService _adjudicacionService;
        public AdjudicacionControlador(IAdjudicacionService adjudicacionService) => _adjudicacionService = adjudicacionService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _adjudicacionService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _adjudicacionService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdjudicacionDTO dto)
        {
            await _adjudicacionService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAdjudicacionDTO dto)
        {
            await _adjudicacionService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _adjudicacionService.DeleteAsync(id);
            return Ok();
        }
    }
}