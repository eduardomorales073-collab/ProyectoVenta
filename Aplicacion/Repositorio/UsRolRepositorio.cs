using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface UsRolRepositorio
    {
        Task<List< Usuarios_Roles>> GetAllasync();
        Task<Usuarios_Roles> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Usuarios_Roles usuariosRoles);
        Task UpdateAsync(Usuarios_Roles usuariosRoles);
    }
}
