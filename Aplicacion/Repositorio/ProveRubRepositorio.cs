using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;
using Aplicacion.modelos;

namespace Aplicacion.Repositorio
{
    public interface ProveRubRepositorio
    {
        Task<List<Provee_Rubro>> GetAllasync();
        Task<Provee_Rubro> GetAsync(int idProveedor, int idRubro);
        Task DeletAsync(int idProveedor, int idRubro);
        Task AddAsync(Provee_Rubro proveeRubro);
        Task UpdateAsync(Provee_Rubro proveeRubro);

        // ✅ NUEVO: Eliminar todas las asociaciones de un proveedor
        Task EliminarPorProveedorAsync(int idProveedor);
    }
}