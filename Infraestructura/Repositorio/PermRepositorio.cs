using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class PermRepositorio : PermisosRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public PermRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Permisos permisos)
        {
            await _context.Permisos.AddAsync(permisos);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var permisos = await _context.Permisos.FindAsync(id);
            if (permisos != null)
            {
                _context.Permisos.Remove(permisos);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Permisos>> GetAllasync()
        {
            return await _context.Permisos.ToListAsync();
        }

        public async Task<Permisos> GetAsync(int id)
        {
            var permisos = await _context.Permisos.FindAsync(id);
            return permisos!;
        }

        public async Task UpdateAsync(Permisos permisos)
        {
            _context.Permisos.Update(permisos);
            await _context.SaveChangesAsync();
        }
    }
}
