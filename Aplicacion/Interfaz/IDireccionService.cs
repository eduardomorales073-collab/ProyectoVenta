using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IDireccionService
    {
        Task<List<DireccionDTO>> GetAllsync();
        Task<DireccionDTO> GetByIdAsync(int id);
        Task AddAsync(CreateDireccionDTO direccion);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateDireccionDTO direccion);
    }
}
