using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface DetaTelRepositorio
    {
        Task<List<Detalle_Telefono>> GetAllsync();
        Task<List<Detalle_Telefono>> GetBySucursalAsync(int idSucursal);
        Task<Detalle_Telefono> GetAsync(int idSucursal, int idTelefono);
        Task AddAsync(Detalle_Telefono detalle);
        Task DeleteAsync(int idSucursal, int idTelefono);
        Task DeleteBySucursalAsync(int idSucursal);
    }
}