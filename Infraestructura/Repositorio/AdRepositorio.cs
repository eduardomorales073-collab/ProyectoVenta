using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class AdRepositorio : AdjuRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public AdRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Adjudicacion adjudicacion)
        {
            await _context.Adjudicacion.AddAsync(adjudicacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var adjudicacion = await _context.Adjudicacion.FindAsync(id);
            if (adjudicacion != null)
            {
                _context.Adjudicacion.Remove(adjudicacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Adjudicacion>> GetAllasync()
        {
            return await _context.Adjudicacion.ToListAsync();
        }

        public  async Task<Adjudicacion> GetAsync(int id)
        {
            var adjudicacion = await _context.Adjudicacion.FindAsync(id);
            return adjudicacion!;
        }

        public async Task UpdateAsync(Adjudicacion adjudicacion)
        {
            _context.Adjudicacion.Update(adjudicacion);
            await _context.SaveChangesAsync();
        }
    }
}
