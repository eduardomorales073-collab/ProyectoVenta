using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class RolRepositorio : RolesRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public RolRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Roles roles)
        {
            await _context.Roles.AddAsync(roles);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var roles = await _context.Roles.FindAsync(id);
            if (roles != null)
            {
                _context.Roles.Remove(roles);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Roles>> GetAllasync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Roles> GetAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task UpdateAsync(Roles roles)
        {
            _context.Roles.Update(roles);
            await _context.SaveChangesAsync();
        }
    }
}
