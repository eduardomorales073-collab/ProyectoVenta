using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ← Solo requiere autenticación, NO rol específico
    public class OfertaProveedorControlador : ControllerBase
    {
        private readonly IOfeProveService _ofeProveService;

        public OfertaProveedorControlador(IOfeProveService ofeProveService) => _ofeProveService = ofeProveService;

        [HttpGet]
        [Authorize(Roles = "Administrador,GestorCompras")]
        public async Task<IActionResult> GetAll() => Ok(await _ofeProveService.GetAllsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,GestorCompras,AdministradorProveedor")]
        public async Task<IActionResult> GetById(int id) => Ok(await _ofeProveService.GetByIdAsync(id));

        // Endpoint para que el Administrador de Proveedor vea SOLO sus ofertas
        [HttpGet("mis-ofertas")]
        [Authorize(Roles = "AdministradorProveedor,Administrador")]
        public async Task<IActionResult> MisOfertas()
        {
            var idProveedorClaim = User.FindFirst("IdProveedor")?.Value;

            if (string.IsNullOrEmpty(idProveedorClaim))
            {
                if (User.IsInRole("Administrador"))
                {
                    return Ok(await _ofeProveService.GetAllsync());
                }
                return Ok(new List<OfertaProveedorDTO>());
            }

            var idProveedor = int.Parse(idProveedorClaim);
            return Ok(await _ofeProveService.GetByProveedorAsync(idProveedor));
        }

        // Endpoint para crear ofertas (solo Admin de Proveedor)
        [HttpPost("ofertar")]
        [Authorize(Roles = "AdministradorProveedor")]
        public async Task<IActionResult> Ofertar(CreateOfertaProveedorDTO dto)
        {
            var idProveedorClaim = User.FindFirst("IdProveedor")?.Value;
            if (string.IsNullOrEmpty(idProveedorClaim))
            {
                return Unauthorized(new { mensaje = "No tienes un proveedor asignado." });
            }

            var idProveedor = int.Parse(idProveedorClaim);
            if (dto.id_Proveedor != idProveedor)
            {
                return Forbid();
            }

            await _ofeProveService.AddAsync(dto);
            return Ok(new { mensaje = "Oferta registrada correctamente." });
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(CreateOfertaProveedorDTO dto)
        {
            await _ofeProveService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Administrador,AdministradorProveedor")]
        public async Task<IActionResult> Update(UpdateOfertaProveedorDTO dto)
        {
            await _ofeProveService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,AdministradorProveedor")]
        public async Task<IActionResult> Delete(int id)
        {
            await _ofeProveService.DeleteAsync(id);
            return Ok();
        }
    }
}