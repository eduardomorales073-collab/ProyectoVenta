using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticuloControlador : ControllerBase
    {
        private readonly IArticuloService _articuloService;
        public ArticuloControlador(IArticuloService articuloService)
        {
            _articuloService = articuloService;
        }
        [HttpGet]
        public async Task <IActionResult> GetAll() => Ok(await _articuloService.GetAllsync());

        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id) => Ok(await _articuloService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateArticuloDTO dto)
        {
            await _articuloService.AddAsync(dto);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Update(UpdateActArtDTO dto)
        {
            await _articuloService.UpdateAsync(dto);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _articuloService.DeleteAsync(id);
            return Ok();
        }
    }
}
