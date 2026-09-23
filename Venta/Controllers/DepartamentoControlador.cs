// DepartamentoControlador.cs
using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentoControlador : ControllerBase
    {
        private readonly IDepartaentoService _departamentoService;
        public DepartamentoControlador(IDepartaentoService departamentoService) => _departamentoService = departamentoService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _departamentoService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _departamentoService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartamentoDTO dto)
        {
            await _departamentoService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateDepartamentoDTO dto)
        {
            await _departamentoService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departamentoService.DeleteAsync(id);
            return Ok();
        }
    }
}