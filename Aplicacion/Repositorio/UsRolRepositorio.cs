using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface UsRolRepositorio
    {
        Task<List<Usuarios_Roles>> GetAllasync();
        Task<Usuarios_Roles> GetAsync(int idUsuario, int idRol);
        Task DeletAsync(int idUsuario, int idRol);
        Task AddAsync(Usuarios_Roles usuariosRoles);
        Task UpdateAsync(Usuarios_Roles usuariosRoles);
    }
}