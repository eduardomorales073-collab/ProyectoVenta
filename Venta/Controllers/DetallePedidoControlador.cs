// DetallePedidoControlador.cs (2 claves)
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallePedidoControlador : ControllerBase
    {
        private readonly IDetPedidoService _detPedidoService;
        public DetallePedidoControlador(IDetPedidoService detPedidoService) => _detPedidoService = detPedidoService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _detPedidoService.GetAllsync());

        [HttpGet("{idPedido}/{idArticulo}")]
        public async Task<IActionResult> GetById(int idPedido, int idArticulo) =>
            Ok(await _detPedidoService.GetByIdAsync(idPedido, idArticulo));

        [HttpPost]
        public async Task<IActionResult> Create(CreateDetallePedidoDTO dto)
        {
            await _detPedidoService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDetallePedidoDTO dto)
        {
            await _detPedidoService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{idPedido}/{idArticulo}")]
        public async Task<IActionResult> Delete(int idPedido, int idArticulo)
        {
            await _detPedidoService.DeleteAsync(idPedido, idArticulo);
            return Ok();
        }
    }
}