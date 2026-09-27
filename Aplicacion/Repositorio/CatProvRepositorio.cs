using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface CatProvRepositorio
    {
        Task<List<Categoria_Proveedor>> GetAllasync();
        Task<Categoria_Proveedor> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Categoria_Proveedor categoria);
        Task UpdateAsync(Categoria_Proveedor categoria);
    }
}