using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface RelacionRepositorio
    {
        Task<List<Relacion>> GetAllasync();
        Task<Relacion> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Relacion relacion);
        Task UpdateAsync(Relacion relacion);
    }
}
