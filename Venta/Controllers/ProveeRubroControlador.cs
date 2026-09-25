// ProveeRubroControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveeRubroControlador : ControllerBase
    {
        private readonly IProvRubService _provRubService;

        public ProveeRubroControlador(IProvRubService provRubService) => _provRubService = provRubService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _provRubService.GetAllsync());

        // ✅ CAMBIO: 2 parámetros en lugar de 1
        [HttpGet("{idProveedor}/{idRubro}")]
        public async Task<IActionResult> GetById(int idProveedor, int idRubro)
            => Ok(await _provRubService.GetByIdAsync(idProveedor, idRubro));

        [HttpPost]
        public async Task<IActionResult> Create(CreateProveeRubroDTO dto)
        {
            await _provRubService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProveeRubroDTO dto)
        {
            await _provRubService.UpdateAsync(dto);
            return Ok();
        }

        // ✅ CAMBIO: 2 parámetros en lugar de 1
        [HttpDelete("{idProveedor}/{idRubro}")]
        public async Task<IActionResult> Delete(int idProveedor, int idRubro)
        {
            await _provRubService.DeleteAsync(idProveedor, idRubro);
            return Ok();
        }

        // ✅ NUEVO: Eliminar todas las asociaciones de un proveedor
        [HttpDelete("por-proveedor/{idProveedor}")]
        public async Task<IActionResult> DeleteByProveedor(int idProveedor)
        {
            await _provRubService.EliminarPorProveedorAsync(idProveedor);
            return Ok();
        }
    }
}