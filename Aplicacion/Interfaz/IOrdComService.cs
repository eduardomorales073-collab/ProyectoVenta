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
        Task AddAsync(CreateOrdenCompraDTO orden);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateOrdenCompraDTO orden);
    }
}
