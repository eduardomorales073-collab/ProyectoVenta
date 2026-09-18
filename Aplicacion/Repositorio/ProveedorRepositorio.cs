using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface ProveedorRepositorio
    {
        Task<List<Proveedor>> GetAllasync();
        Task<Proveedor> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Proveedor proveedor);
        Task UpdateAsync(Proveedor proveedor);
    }
}
