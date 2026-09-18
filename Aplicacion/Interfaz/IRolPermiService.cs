using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IRolPermiService
    {
        Task<List<RolPermisoDTO>> GetAllsync();
        Task<RolPermisoDTO> GetByIdAsync(int id);
        Task AddAsync(CreateRolPermisoDTO rolPermiso);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateRolPermisoDTO rolPermiso);
    }
}
