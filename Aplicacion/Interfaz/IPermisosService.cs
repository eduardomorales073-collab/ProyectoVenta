using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IPermisosService
    {
        Task<List<PermisosDTO>> GetAllsync();
        Task<PermisosDTO> GetByIdAsync(int id);
        Task AddAsync(CreatePermisosDTO permiso);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdatePermisosDTO permiso);
    }
}
