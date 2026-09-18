using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class UsuarRepositorio : UsuarioRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public UsuarRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Usuarios usuarios)
        {
            await _context.Usuarios.AddAsync(usuarios);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Usuarios>> GetAllasync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuarios> GetAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task UpdateAsync(Usuarios usuarios)
        {
            _context.Usuarios.Update(usuarios);
            await _context.SaveChangesAsync();
        }
    }
}
