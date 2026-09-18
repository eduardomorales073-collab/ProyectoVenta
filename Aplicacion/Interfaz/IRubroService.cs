using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IRubroService
    {
        Task<List<RubroDTO>> GetAllsync();
        Task<RubroDTO> GetByIdAsync(int id);
        Task AddAsync(CreateRubroDTO rubro);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateRubroDTO rubro);
    }
}
