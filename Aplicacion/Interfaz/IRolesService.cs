using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IRolesService
    {
        Task<List<RolesDTO>> GetAllsync();
        Task<RolesDTO> GetByIdAsync(int id);
        Task AddAsync(CreateRolesDTO roles);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateRolesDTO roles);
    }
}
