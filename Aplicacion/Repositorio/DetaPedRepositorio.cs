using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface DetaPedRepositorio
    {
        Task<List<Detalle_Pedido>> GetAllasync();
        Task<Detalle_Pedido> GetAsync(int idPedido, int idArticulo);
        Task DeletAsync(int idPedido, int idArticulo);
        Task AddAsync(Detalle_Pedido detallePedido);
        Task UpdateAsync(Detalle_Pedido detallePedido);
    }
}
