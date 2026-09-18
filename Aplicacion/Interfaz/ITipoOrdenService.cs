using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface ITipoOrdenService
    {
        Task<List<TipoOrdenDTO>> GetAllsync();
        Task<TipoOrdenDTO> GetByIdAsync(int id);
        Task AddAsync(CreateTipoOrdenDTO tipoOrden);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateTipoOrdenDTO tipoOrden);
    }
}
