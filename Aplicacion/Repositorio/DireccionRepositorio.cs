using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface DireccionRepositorio
    {
        Task<List<Direccion>> GetAllasync();
        Task<Direccion> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Direccion direccion);
        Task UpdateAsync(Direccion direccion);
    }
}
