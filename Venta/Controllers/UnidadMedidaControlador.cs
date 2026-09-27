using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UnidadMedidaControlador : ControllerBase
    {
        private readonly IUnidadMedidaService _unidadService;

        public UnidadMedidaControlador(IUnidadMedidaService unidadService)
        {
            _unidadService = unidadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _unidadService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _unidadService.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Create(CreateUnidadMedidaDTO dto)
        {
            await _unidadService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Update(UpdateUnidadMedidaDTO dto)
        {
            await _unidadService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            await _unidadService.DeleteAsync(id);
            return Ok();
        }
    }
}