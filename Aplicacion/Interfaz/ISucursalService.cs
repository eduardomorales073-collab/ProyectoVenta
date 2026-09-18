using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface ISucursalRepositorio
    {
        Task<List<SucursalDTO>> GetAllsync();
        Task<SucursalDTO> GetByIdAsync(int id);
        Task AddAsync(CreateSucursalDTO sucursal);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateSucursalDTO sucursal);
    }
}
