using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PedidoInternoControlador : ControllerBase
    {
        private readonly IPedIntService _pedIntService;

        public PedidoInternoControlador(IPedIntService pedIntService) => _pedIntService = pedIntService;

        [HttpGet]
        [Authorize(Roles = "Administrador,GestorCompras,CreadorPedidos,AdministradorProveedor")]
        public async Task<IActionResult> GetAll() => Ok(await _pedIntService.GetAllsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,GestorCompras,CreadorPedidos")]
        public async Task<IActionResult> GetById(int id) => Ok(await _pedIntService.GetByIdAsync(id));

        // ← Solo Admin, Gestor y Creador de Pedidos pueden CREAR
        [HttpPost]
        [Authorize(Roles = "Administrador,GestorCompras,CreadorPedidos")]
        public async Task<IActionResult> Create(CreatePedidoInternoDTO dto)
        {
            await _pedIntService.AddAsync(dto);
            return Ok();
        }

        // ← Solo Admin y Gestor pueden ACTUALIZAR (Creador NO)
        [HttpPut]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Update(UpdatePedidoInternoDTO dto)
        {
            await _pedIntService.UpdateAsync(dto);
            return Ok();
        }

        // ← Solo Admin puede ELIMINAR
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pedIntService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("por-departamento/{idDepartamento}")]
        [Authorize(Roles = "Administrador,GestorCompras,CreadorPedidos")]
        public async Task<IActionResult> GetByDepartamento(int idDepartamento)
        {
            var pedidos = await _pedIntService.GetByDepartamentoAsync(idDepartamento);
            return Ok(pedidos);
        }
    }
}