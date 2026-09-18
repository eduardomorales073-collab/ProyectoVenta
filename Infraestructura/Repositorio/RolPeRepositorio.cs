using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    internal class RolPeRepositorio : RolPerRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public RolPeRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Rol_Permiso rolPermiso)
        {
            await _context.Rol_Permiso.AddAsync(rolPermiso);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var rolPermiso = await _context.Rol_Permiso.FindAsync(id);
            if (rolPermiso != null)
            {
                _context.Rol_Permiso.Remove(rolPermiso);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Rol_Permiso>> GetAllasync()
        {
            return await _context.Rol_Permiso.ToListAsync();
        }

        public async Task<Rol_Permiso> GetAsync(int id)
        {
            return await _context.Rol_Permiso.FindAsync(id);
        
        }

        public async Task UpdateAsync(Rol_Permiso rolPermiso)
        {
            _context.Rol_Permiso.Update(rolPermiso);
            await _context.SaveChangesAsync();
        }
    }
}
