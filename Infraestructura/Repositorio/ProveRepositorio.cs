using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class ProveRepositorio : ProveedorRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public ProveRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Proveedor proveedor)
        {
            await _context.Proveedor.AddAsync(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedor.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Proveedor>> GetAllasync()
        {
            return await _context.Proveedor.ToListAsync();
        }

        public async Task<Proveedor> GetAsync(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            return proveedor!;
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            _context.Proveedor.Update(proveedor);
            await _context.SaveChangesAsync();
        }
    }
}