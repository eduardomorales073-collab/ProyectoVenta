using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IDetAdjuService
    {
        Task<List<DetalleAdjudicacionDTO>> GetAllsync();
        Task<DetalleAdjudicacionDTO> GetByIdAsync(int idAdjudicacion, int idPedido, int idProveedor);
        Task AddAsync(CreateDetalleAdjudicacionDTO detalle);
        Task DeleteAsync(int idAdjudicacion, int idPedido, int idProveedor);
        Task UpdateAsync(UpdateDetalleAdjudicacionDTO detalle);
    }
}
