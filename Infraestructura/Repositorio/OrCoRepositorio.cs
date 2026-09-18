using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    internal class OrCoRepositorio : OrdComRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public OrCoRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Orden_Compra ordenCompra)
        {
            await _context.Orden_Compra.AddAsync(ordenCompra);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var ordenCompra = await _context.Orden_Compra.FindAsync(id);
            if (ordenCompra != null)
            {
                _context.Orden_Compra.Remove(ordenCompra);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Orden_Compra>> GetAllasync()
        {
            return await _context.Orden_Compra.ToListAsync();
        }

        public async Task<Orden_Compra> GetAsync(int id)
        {
            var ordenCompra = await _context.Orden_Compra.FindAsync(id);
            return ordenCompra!;
        }

        public async Task UpdateAsync(Orden_Compra ordenCompra)
        {
            _context.Orden_Compra.Update(ordenCompra);
            await _context.SaveChangesAsync();
        }
    }
}
