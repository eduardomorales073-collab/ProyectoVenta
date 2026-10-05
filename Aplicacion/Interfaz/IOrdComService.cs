using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IOrdComService
    {
        Task<List<OrdenCompraDTO>> GetAllsync();
        Task<OrdenCompraDTO> GetByIdAsync(int id);
        Task AddAsync(CreateOrdenCompraDTO orden, int idUsuario, int idDepartamento);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateOrdenCompraDTO orden, int idUsuario, string rol);

        Task<List<OrdenCompraConContadoresDTO>> GetConContadoresAsync(
            int idUsuario, string rol, int? idDepartamento);

        Task<List<PedidoInternoDTO>> GetPedidosDeOrdenAsync(int idOrden);

        // ← Estados
        Task AprobarAsync(int id);      
        Task PublicarAsync(int id);     
        Task CerrarAsync(int id);       
        Task CancelarAsync(int id);
    }
}