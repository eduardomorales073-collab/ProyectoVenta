using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface ProveRubRepositorio
    {
        Task<List<Provee_Rubro>> GetAllasync();
        Task<Provee_Rubro> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Provee_Rubro proveeRubro);
        Task UpdateAsync(Provee_Rubro proveeRubro);
    }
}
