using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class TelRepositorio : TelefonoRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public TelRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Telefono telefono)
        {
            await _context.Telefono.AddAsync(telefono);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var telefono = await _context.Telefono.FindAsync(id);
            if (telefono != null)
            {
                _context.Telefono.Remove(telefono);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Telefono>> GetAllasync()
        {
            return await _context.Telefono.ToListAsync();
        }

        public async Task<Telefono> GetAsync(int id)
        {
            return await _context.Telefono.FindAsync(id);
        }

        public async Task UpdateAsync(Telefono telefono)
        {
            _context.Telefono.Update(telefono);
            await _context.SaveChangesAsync();
        }
    }
}
