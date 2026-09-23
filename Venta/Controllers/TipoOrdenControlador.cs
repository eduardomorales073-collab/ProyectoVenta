// TipoOrdenControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoOrdenControlador : ControllerBase
    {
        private readonly ITipoOrdenService _tipoOrdenService;
        public TipoOrdenControlador(ITipoOrdenService tipoOrdenService) => _tipoOrdenService = tipoOrdenService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _tipoOrdenService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _tipoOrdenService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateTipoOrdenDTO dto)
        {
            await _tipoOrdenService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateTipoOrdenDTO dto)
        {
            await _tipoOrdenService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _tipoOrdenService.DeleteAsync(id);
            return Ok();
        }
    }
}