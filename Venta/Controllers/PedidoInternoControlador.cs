// PedidoInternoControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoInternoControlador : ControllerBase
    {
        private readonly IPedIntService _pedIntService;
        public PedidoInternoControlador(IPedIntService pedIntService) => _pedIntService = pedIntService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _pedIntService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _pedIntService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreatePedidoInternoDTO dto)
        {
            await _pedIntService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePedidoInternoDTO dto)
        {
            await _pedIntService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pedIntService.DeleteAsync(id);
            return Ok();
        }
    }
}