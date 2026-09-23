// DetalleAdjudicacionControlador.cs (3 claves)
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleAdjudicacionControlador : ControllerBase
    {
        private readonly IDetAdjuService _detAdjuService;
        public DetalleAdjudicacionControlador(IDetAdjuService detAdjuService) => _detAdjuService = detAdjuService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _detAdjuService.GetAllsync());

        [HttpGet("{idAdjudicacion}/{idPedido}/{idProveedor}")]
        public async Task<IActionResult> GetById(int idAdjudicacion, int idPedido, int idProveedor) =>
            Ok(await _detAdjuService.GetByIdAsync(idAdjudicacion, idPedido, idProveedor));

        [HttpPost]
        public async Task<IActionResult> Create(CreateDetalleAdjudicacionDTO dto)
        {
            await _detAdjuService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDetalleAdjudicacionDTO dto)
        {
            await _detAdjuService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{idAdjudicacion}/{idPedido}/{idProveedor}")]
        public async Task<IActionResult> Delete(int idAdjudicacion, int idPedido, int idProveedor)
        {
            await _detAdjuService.DeleteAsync(idAdjudicacion, idPedido, idProveedor);
            return Ok();
        }
    }
}