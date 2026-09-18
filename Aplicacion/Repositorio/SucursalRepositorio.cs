using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface SucursalRepositorio
    {
        Task<List<Sucursal>> GetAllasync();
        Task<Sucursal> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Sucursal sucursal);
        Task UpdateAsync(Sucursal sucursal);
    }
}
