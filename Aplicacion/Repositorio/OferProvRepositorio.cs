using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface OferProvRepositorio
    {
        Task<List<Oferta_Proveedor>> GetAllasync();
        Task<Oferta_Proveedor> GetAsync(int id);
        Task DeletAsync(int id);
        Task AddAsync(Oferta_Proveedor ofertaProveedor);
        Task UpdateAsync(Oferta_Proveedor ofertaProveedor);
    }
}
