using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface TipoOrRepositorio
    {
        Task<List<Tipo_Orden>> GetAllasync();
        Task<Tipo_Orden> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Tipo_Orden tipoOrden);
        Task UpdateAsync(Tipo_Orden tipoOrden);
    }
}
