using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IRelacionService
    {
        Task<List<RelacionDTO>> GetAllsync();
        Task<RelacionDTO> GetByIdAsync(int id);
        Task AddAsync(CreateRelacionDTO relacion);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateRelacionDTO relacion);
    }
}
