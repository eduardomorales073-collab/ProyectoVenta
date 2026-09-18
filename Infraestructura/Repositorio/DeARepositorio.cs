using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class DeARepositorio : DetaAdjRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public DeARepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Detalle_Adjudicacion detalleAdjudicacion)
        {
            await _context.Detalle_Adjudicacion.AddAsync(detalleAdjudicacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int idAdjudicacion, int idPedido, int idProveedor)
        {
            var detalleAdjudicacion = await _context.Detalle_Adjudicacion
                .FindAsync(idAdjudicacion, idPedido, idProveedor);
            if (detalleAdjudicacion != null)
            {
                _context.Detalle_Adjudicacion.Remove(detalleAdjudicacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Detalle_Adjudicacion>> GetAllasync()
        {
            return await _context.Detalle_Adjudicacion.ToListAsync();
        }

        public async Task<Detalle_Adjudicacion> GetAsync(int idAdjudicacion, int idPedido, int idProveedor)
        {
            var detalleAdjudicacion = await _context.Detalle_Adjudicacion
                .FindAsync(idAdjudicacion, idPedido, idProveedor);
            return detalleAdjudicacion!;
        }

        public async Task UpdateAsync(Detalle_Adjudicacion detalleAdjudicacion)
        {
            _context.Detalle_Adjudicacion.Update(detalleAdjudicacion);
            await _context.SaveChangesAsync();
        }
    }
}