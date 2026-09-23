// DetalleTelefonoControlador.cs (2 claves)
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleTelefonoControlador : ControllerBase
    {
        private readonly IDetTelefonoService _detTelefonoService;
        public DetalleTelefonoControlador(IDetTelefonoService detTelefonoService) => _detTelefonoService = detTelefonoService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _detTelefonoService.GetAllsync());

        [HttpGet("{idSucursal}/{idTelefono}")]
        public async Task<IActionResult> GetById(int idSucursal, int idTelefono) =>
            Ok(await _detTelefonoService.GetByIdAsync(idSucursal, idTelefono));

        [HttpPost]
        public async Task<IActionResult> Create(CreateDetalleTelefonoDTO dto)
        {
            await _detTelefonoService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDetalleTelefonoDTO dto)
        {
            await _detTelefonoService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{idSucursal}/{idTelefono}")]
        public async Task<IActionResult> Delete(int idSucursal, int idTelefono)
        {
            await _detTelefonoService.DeleteAsync(idSucursal, idTelefono);
            return Ok();
        }
    }
}