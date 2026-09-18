using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface PermisosRepositorio
    {
        Task<List<Permisos>> GetAllasync();
        Task<Permisos> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Permisos permisos);
        Task UpdateAsync(Permisos permisos);
    }
}
