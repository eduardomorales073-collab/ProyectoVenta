using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class OfeProRepositorio : OferProvRepositorio // ajusta al nombre real de la interfaz
    {
        private readonly AplicacionDBContexto _context;
        public OfeProRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Oferta_Proveedor oferta)
        {
            await _context.Oferta_Proveedor.AddAsync(oferta);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var oferta = await _context.Oferta_Proveedor.FindAsync(id);
            if (oferta != null)
            {
                _context.Oferta_Proveedor.Remove(oferta);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Oferta_Proveedor>> GetAllasync()
        {
            return await _context.Oferta_Proveedor.ToListAsync();
        }

        public async Task<Oferta_Proveedor> GetAsync(int id)
        {
            var oferta = await _context.Oferta_Proveedor.FindAsync(id);
            return oferta!;
        }

        public async Task UpdateAsync(Oferta_Proveedor oferta)
        {
            _context.Oferta_Proveedor.Update(oferta);
            await _context.SaveChangesAsync();
        }
    }
}