using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface DepartaRepositorio
    {
        Task<List<Departamento>> GetAllasync();
        Task<Departamento> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Departamento departamento);
        Task UpdateAsync(Departamento departamento);
    }
}
