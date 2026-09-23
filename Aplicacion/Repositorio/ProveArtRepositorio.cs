using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface ProveArtRepositorio
    {
        Task<List<Provee_Artic>> GetAllasync();
        Task<Provee_Artic> GetAsync(int idProveedor, int idArticulo);
        Task DeletAsync(int idProveedor, int idArticulo);
        Task AddAsync(Provee_Artic proveeArtic);
        Task UpdateAsync(Provee_Artic proveeArtic);
    }
}
