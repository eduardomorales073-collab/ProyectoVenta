using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    internal class RelaRepositorio: RelacionRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public RelaRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Relacion relacion)
        {
            await _context.Relacion.AddAsync(relacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var relacion = await _context.Relacion.FindAsync(id);
            if (relacion != null)
            {
                _context.Relacion.Remove(relacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Relacion>> GetAllasync()
        {
            return await _context.Relacion.ToListAsync();
        }

        public async Task<Relacion> GetAsync(int id)
        {
            return await _context.Relacion.FindAsync(id);
        }

        public async Task UpdateAsync(Relacion relacion)
        {
            _context.Relacion.Update(relacion);
            await _context.SaveChangesAsync();
        }
    }
}
