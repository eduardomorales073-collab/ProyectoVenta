using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IOfeProveService
    {
        Task<List<OfertaProveedorDTO>> GetAllsync();
        Task<OfertaProveedorDTO> GetByIdAsync(int id);
        Task AddAsync(CreateOfertaProveedorDTO oferta);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateOfertaProveedorDTO oferta);
    }
}
