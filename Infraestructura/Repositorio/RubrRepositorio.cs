using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    internal class RubrRepositorio : RubroRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public RubrRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Rubro rubro)
        {
            await _context.Rubro.AddAsync(rubro);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var rubro = await _context.Rubro.FindAsync(id);
            if (rubro != null)
            {
                _context.Rubro.Remove(rubro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Rubro>> GetAllasync()
        {
            return await _context.Rubro.ToListAsync();
        }

        public async Task<Rubro> GetAsync(int id)
        {
            return await _context.Rubro.FindAsync(id);
        }

        public async Task UpdateAsync(Rubro rubro)
        {
            _context.Rubro.Update(rubro);
            await _context.SaveChangesAsync();
        }
    }
}
