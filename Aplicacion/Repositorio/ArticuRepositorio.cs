using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Repositorio
{
    public interface ArticuRepositorio 

    {
        Task<List<Articulo>> GetAllasync();
        Task<Articulo> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Articulo articulo);
        Task UpdateAsync(Articulo articulo);
    }
}
