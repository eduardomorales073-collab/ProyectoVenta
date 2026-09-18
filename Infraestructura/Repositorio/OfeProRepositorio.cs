using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    internal class OfeProRepositorio : OferProvRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public OfeProRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Oferta_Proveedor ofertaProveedor)
        {
            await _context.Oferta_Proveedor.AddAsync(ofertaProveedor);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var ofertaProveedor = await _context.Oferta_Proveedor.FindAsync(id);
            if (ofertaProveedor != null)
            {
                _context.Oferta_Proveedor.Remove(ofertaProveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Oferta_Proveedor>> GetAllasync()
        {
            throw new NotImplementedException();
        }

        public async Task<Oferta_Proveedor> GetAsync(int id)
        {
            var ofertaProveedor = await _context.Oferta_Proveedor.FindAsync(id);
            return ofertaProveedor!;
        }

        public async Task UpdateAsync(Oferta_Proveedor ofertaProveedor)
        {
            _context.Oferta_Proveedor.Update(ofertaProveedor);
            await _context.SaveChangesAsync();
        }
    }
}
