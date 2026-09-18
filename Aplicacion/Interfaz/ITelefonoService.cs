using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface ITelefonoService
    {
        Task<List<TelefonoDTO>> GetAllsync();
        Task<TelefonoDTO> GetByIdAsync(int id);
        Task AddAsync(CreateTelefonoDTO telefono);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateTelefonoDTO telefono);
    }
}
