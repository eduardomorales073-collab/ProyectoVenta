using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IProvRubService
    {
        Task<List<ProveeRubroDTO>> GetAllsync();
        Task<ProveeRubroDTO> GetByIdAsync(int id);
        Task AddAsync(CreateProveeRubroDTO proveeRubro);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateProveeRubroDTO proveeRubro);
    }
}
