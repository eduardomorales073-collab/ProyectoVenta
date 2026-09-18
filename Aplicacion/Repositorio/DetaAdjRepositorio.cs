using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Repositorio
{
    public interface DetaAdjRepositorio
    {
        Task<List<Detalle_Adjudicacion>> GetAllasync();
        Task<Detalle_Adjudicacion> GetAsync(int idAdjudicacion, int idPedido, int idProveedor);
        Task DeletAsync(int idAdjudicacion, int idPedido, int idProveedor);
        Task AddAsync(Detalle_Adjudicacion detalleAdjudicacion);
        Task UpdateAsync(Detalle_Adjudicacion detalleAdjudicacion);
    }
}