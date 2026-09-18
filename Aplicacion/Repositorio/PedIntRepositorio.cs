using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface PedIntRepositorio
    {
        Task<List<Pedido_Interno>> GetAllasync();
        Task<Pedido_Interno> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Pedido_Interno pedidoInterno);
        Task UpdateAsync(Pedido_Interno pedidoInterno);
    }
}
