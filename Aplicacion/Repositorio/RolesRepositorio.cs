using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface RolesRepositorio
    {
        Task<List<Roles>> GetAllasync();
        Task<Roles> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Roles roles);
        Task UpdateAsync(Roles roles);
    }
}
