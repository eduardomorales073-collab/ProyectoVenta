using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,GestorCompras")]
    public class RelacionControlador : ControllerBase
    {
        private readonly IRelacionService _relacionService;
        public RelacionControlador(IRelacionService relacionService) => _relacionService = relacionService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _relacionService.GetAllsync());

        [HttpPost]
        public async Task<IActionResult> Create(CreateRelacionDTO dto)
        {
            await _relacionService.AddAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _relacionService.DeleteAsync(id);
            return Ok();
        }
    }
}