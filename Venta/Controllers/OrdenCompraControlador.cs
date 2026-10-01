using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenCompraControlador : ControllerBase
    {
        private readonly IOrdComService _ordComService;
        public OrdenCompraControlador(IOrdComService ordComService) => _ordComService = ordComService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _ordComService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _ordComService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrdenCompraDTO dto)
        {
            await _ordComService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateOrdenCompraDTO dto)
        {
            await _ordComService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ordComService.DeleteAsync(id);
            return Ok();
        }

        // ==================== NUEVOS ENDPOINTS ====================

        [HttpGet("con-contadores")]
        public async Task<IActionResult> GetConContadores()
            => Ok(await _ordComService.GetConContadoresAsync());

        [HttpGet("{id}/pedidos")]
        public async Task<IActionResult> GetPedidosDeOrden(int id)
            => Ok(await _ordComService.GetPedidosDeOrdenAsync(id));
    }
}