using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class SucurRepositorio : SucursalRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public SucurRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Sucursal sucursal)
        {
            await _context.Sucursal.AddAsync(sucursal);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var sucursal = await _context.Sucursal.FindAsync(id);
            if (sucursal != null)
            {
                _context.Sucursal.Remove(sucursal);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Sucursal>> GetAllasync()
        {
            return await _context.Sucursal.ToListAsync();
        }

        public async Task<Sucursal> GetAsync(int id)
        {
            return await _context.Sucursal.FindAsync(id);
        }

        public async Task UpdateAsync(Sucursal sucursal)
        {
            _context.Sucursal.Update(sucursal);
            await _context.SaveChangesAsync();
        }
    }
}
