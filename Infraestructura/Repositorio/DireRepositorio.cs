using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class DireRepositorio : DireccionRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public DireRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Direccion direccion)
        {
            await _context.Direccion.AddAsync(direccion);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var direccion = await _context.Direccion.FindAsync(id);
            if (direccion != null)
            {
                _context.Direccion.Remove(direccion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Direccion>> GetAllasync()
        {
            return await _context.Direccion.ToListAsync();
        }

        public async Task<Direccion> GetAsync(int id)
        {
            var direccion = await _context.Direccion.FindAsync(id);
            return direccion!;

        }

        public async Task UpdateAsync(Direccion direccion)
        {
            _context.Direccion.Update(direccion);
            await _context.SaveChangesAsync();
        }
    }
}
