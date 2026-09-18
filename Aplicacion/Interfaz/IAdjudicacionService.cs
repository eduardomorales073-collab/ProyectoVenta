using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IAdjudicacionService
    {
        Task<List<AdjudicacionDTO>> GetAllsync();
        Task<AdjudicacionDTO> GetByIdAsync(int id);
        Task AddAsync(CreateAdjudicacionDTO adjudicacion);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateAdjudicacionDTO adjudicacion);
    }
}
