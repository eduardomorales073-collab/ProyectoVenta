using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriaProveedorControlador : ControllerBase
    {
        private readonly ICategoriaProveedorService _categoriaService;

        public CategoriaProveedorControlador(ICategoriaProveedorService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _categoriaService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _categoriaService.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Create(CreateCategoriaProveedorDTO dto)
        {
            await _categoriaService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Update(UpdateCategoriaProveedorDTO dto)
        {
            await _categoriaService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoriaService.DeleteAsync(id);
            return Ok();
        }
    }
}