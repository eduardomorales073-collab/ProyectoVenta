using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface UnidadMedidaRepositorio
    {
        Task<List<Unidad_Medida>> GetAllasync();
        Task<Unidad_Medida> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Unidad_Medida unidad);
        Task UpdateAsync(Unidad_Medida unidad);
    }
}