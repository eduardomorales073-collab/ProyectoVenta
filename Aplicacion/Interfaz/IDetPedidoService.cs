using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IDetPedidoService
    {
        Task<List<DetallePedidoDTO>> GetAllsync();
        Task<DetallePedidoDTO> GetByIdAsync(int idPedido, int idArticulo);
        Task AddAsync(CreateDetallePedidoDTO detalle);
        Task DeleteAsync(int idPedido, int idArticulo);
        Task UpdateAsync(UpdateDetallePedidoDTO detalle);
    }
}
