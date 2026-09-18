using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface AdjuRepositorio
    {
        Task<List<Adjudicacion>> GetAllasync();
        Task<Adjudicacion> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Adjudicacion adjudicacion);
        Task UpdateAsync(Adjudicacion adjudicacion);
    }
}
