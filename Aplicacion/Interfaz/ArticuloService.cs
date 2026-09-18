using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IArticuloService
    {
        Task<List<ArticuloDTO>> GetAllsync();
        Task<ArticuloDTO> GetByIdAsync(int id);
        Task AddAsync(CreateArticuloDTO articulo);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateActArtDTO articulo);
    }
}
