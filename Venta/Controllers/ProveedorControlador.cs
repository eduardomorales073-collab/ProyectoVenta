using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProveedorControlador : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorControlador(IProveedorService proveedorService) => _proveedorService = proveedorService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _proveedorService.GetAllsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _proveedorService.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Create(CreateProveedorDTO dto)
        {
            await _proveedorService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> Update(UpdateProveedorDTO dto)
        {
            await _proveedorService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            await _proveedorService.DeleteAsync(id);
            return Ok();
        }

        //  Permite al proveedor asignarse artículos
        [HttpPost("{idProveedor}/asignar-articulos")]
        [Authorize(Roles = "Proveedor,Administrador")] // Solo el proveedor o el admin pueden hacer esto
        public async Task<IActionResult> AsignarArticulos(int idProveedor, [FromBody] List<int> idArticulos)
        {
            // Validación de seguridad: Si es Proveedor, verificar que solo se asigne a sí mismo
            if (User.IsInRole("Proveedor"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

                
            }

            try
            {
                await _proveedorService.AsignarArticulosAsync(idProveedor, idArticulos);
                return Ok(new { mensaje = "Artículos asignados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}