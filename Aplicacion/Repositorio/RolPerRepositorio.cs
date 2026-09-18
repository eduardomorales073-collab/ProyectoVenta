using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface RolPerRepositorio
    {
        Task<List<Rol_Permiso>> GetAllasync();
        Task<Rol_Permiso> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Rol_Permiso rolPermiso);
        Task UpdateAsync(Rol_Permiso rolPermiso);
    }
}
