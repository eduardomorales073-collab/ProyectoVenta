using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface RubroRepositorio
    {
        Task<List<Rubro>> GetAllasync();
        Task<Rubro> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Rubro rubro);
        Task UpdateAsync(Rubro rubro);
    }
}
