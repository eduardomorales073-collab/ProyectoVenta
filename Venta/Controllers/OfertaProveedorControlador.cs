// OfertaProveedorControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertaProveedorControlador : ControllerBase
    {
        private readonly IOfeProveService _ofeProveService;
        public OfertaProveedorControlador(IOfeProveService ofeProveService) => _ofeProveService = ofeProveService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _ofeProveService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _ofeProveService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateOfertaProveedorDTO dto)
        {
            await _ofeProveService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateOfertaProveedorDTO dto)
        {
            await _ofeProveService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ofeProveService.DeleteAsync(id);
            return Ok();
        }
    }
}