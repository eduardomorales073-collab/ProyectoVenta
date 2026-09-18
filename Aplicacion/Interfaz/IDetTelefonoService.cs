using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IDetTelefonoService
    {
        Task<List<DetalleTelefonoDTO>> GetAllsync();
        Task<DetalleTelefonoDTO> GetByIdAsync(int idSucursal, int idTelefono);
        Task AddAsync(CreateDetalleTelefonoDTO detalle);
        Task DeleteAsync(int idSucursal, int idTelefono);
        Task UpdateAsync(UpdateDetalleTelefonoDTO detalle);
    }
}