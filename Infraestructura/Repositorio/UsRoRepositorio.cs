using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class UsRoRepositorio : UsRolRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public UsRoRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Usuarios_Roles usuariosRoles)
        {
            await _context.Usuarios_Roles.AddAsync(usuariosRoles);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var usuariosRoles = await _context.Usuarios_Roles.FindAsync(id);
            if (usuariosRoles != null)
            {
                _context.Usuarios_Roles.Remove(usuariosRoles);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Usuarios_Roles>> GetAllasync()
        {
            return await _context.Usuarios_Roles.ToListAsync();
        }

        public async Task<Usuarios_Roles> GetAsync(int id)
        {
            return await _context.Usuarios_Roles.FindAsync(id);
        }

        public async Task UpdateAsync(Usuarios_Roles usuariosRoles)
        {
            _context.Usuarios_Roles.Update(usuariosRoles);
            await _context.SaveChangesAsync();
        }
    }
}
