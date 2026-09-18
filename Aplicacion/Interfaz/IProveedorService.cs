using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IProveedorService
    {
        Task<List<ProveedorDTO>> GetAllsync();
        Task<ProveedorDTO> GetByIdAsync(int id);
        Task AddAsync(CreateProveedorDTO proveedor);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateProveedorDTO proveedor   );
    }
}
