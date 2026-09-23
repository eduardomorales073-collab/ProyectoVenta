using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class ProArRepositorio : ProveArtRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public ProArRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Provee_Artic proveeArtic)
        {
            await _context.Provee_Artics.AddAsync(proveeArtic);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int idProveedor, int idArticulo)
        {
            var proveeArtic = await _context.Provee_Artics.FindAsync(idProveedor, idArticulo);
            if (proveeArtic != null)
            {
                _context.Provee_Artics.Remove(proveeArtic);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Provee_Artic>> GetAllasync()
        {
            return await _context.Provee_Artics.ToListAsync();
        }

        public async Task<Provee_Artic> GetAsync(int idProveedor, int idArticulo)
        {
            var proveeArtic = await _context.Provee_Artics.FindAsync(idProveedor, idArticulo);
            return proveeArtic!;
        }

        public async Task UpdateAsync(Provee_Artic proveeArtic)
        {
            _context.Provee_Artics.Update(proveeArtic);
            await _context.SaveChangesAsync();
        }
    }
}
