// ProveeArticControlador.cs (2 claves)
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveeArticControlador : ControllerBase
    {
        private readonly IProvArtService _provArtService;
        public ProveeArticControlador(IProvArtService provArtService) => _provArtService = provArtService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _provArtService.GetAllsync());

        [HttpGet("{idProveedor}/{idArticulo}")]
        public async Task<IActionResult> GetById(int idProveedor, int idArticulo) =>
            Ok(await _provArtService.GetByIdAsync(idProveedor, idArticulo));

        [HttpPost]
        public async Task<IActionResult> Create(CreateProveeArtcDTO dto)
        {
            await _provArtService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProveeArtcDTO dto)
        {
            await _provArtService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{idProveedor}/{idArticulo}")]
        public async Task<IActionResult> Delete(int idProveedor, int idArticulo)
        {
            await _provArtService.DeleteAsync(idProveedor, idArticulo);
            return Ok();
        }
    }
}