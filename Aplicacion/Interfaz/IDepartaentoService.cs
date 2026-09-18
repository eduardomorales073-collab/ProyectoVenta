using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IDepartaentoService
    {
        Task<List<DepartamentoDTO>> GetAllsync();
        Task<DepartamentoDTO> GetByIdAsync(int id);
        Task AddAsync(CreateDepartamentoDTO departamento);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateDepartamentoDTO departamento);
    }
}
