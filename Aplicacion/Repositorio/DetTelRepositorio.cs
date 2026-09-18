using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface DetTelRepositorio
    {
        Task<List<Detalle_Telefono>> GetAllasync();
        Task<Detalle_Telefono> GetAsync(int idSucursal, int idTelefono);
        Task DeletAsync(int idSucursal, int idTelefono);
        Task AddAsync(Detalle_Telefono detalleTelefono);
        Task UpdateAsync(Detalle_Telefono detalleTelefono);
    }
}
