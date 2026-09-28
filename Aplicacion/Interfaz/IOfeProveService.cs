using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaz
{
    public interface IOfeProveService
    {
        Task<List<OfertaProveedorDTO>> GetAllsync();
        Task<OfertaProveedorDTO> GetByIdAsync(int id);
        Task AddAsync(CreateOfertaProveedorDTO oferta);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateOfertaProveedorDTO oferta);

        // ← NUEVO: Método para obtener ofertas de un proveedor específico
        Task<List<OfertaProveedorDTO>> GetByProveedorAsync(int idProveedor);
    }
}