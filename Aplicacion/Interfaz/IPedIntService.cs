using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IPedIntService
    {
        Task<List<PedidoInternoDTO>> GetAllsync();
        Task<PedidoInternoDTO> GetByIdAsync(int id);
        Task AddAsync(CreatePedidoInternoDTO pedido);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdatePedidoInternoDTO pedido);
    }
}
