using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticuloControlador : ControllerBase
    {
        private readonly IArticuloService _articuloService;

        public ArticuloControlador(IArticuloService articuloService)
        {
            _articuloService = articuloService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _articuloService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _articuloService.GetByIdAsync(id));

        //  Obtener artículos filtrados por Rubro
        // Útil para que el proveedor vea solo los artículos que puede vender
        [HttpGet("por-rubro/{idRubro}")]
        [Authorize(Roles = "Proveedor,Administrador,GestorCompras")]
        public async Task<IActionResult> GetByRubro(int idRubro)
        {
            try
            {
                var articulos = await _articuloService.GetByRubroAsync(idRubro);
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<IActionResult> Create(CreateArticuloDTO dto)
        {
            await _articuloService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<IActionResult> Update(UpdateActArtDTO dto)
        {
            await _articuloService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            await _articuloService.DeleteAsync(id);
            return Ok();
        }
    }
}