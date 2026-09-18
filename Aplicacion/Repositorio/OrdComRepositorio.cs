using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface OrdComRepositorio
    {
        Task<List<Orden_Compra>> GetAllasync();
        Task<Orden_Compra> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Orden_Compra ordenCompra);
        Task UpdateAsync(Orden_Compra ordenCompra);
    }
}
