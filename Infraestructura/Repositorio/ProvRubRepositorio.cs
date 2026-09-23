using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class ProvRubRepositorio : ProveRubRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public ProvRubRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Provee_Rubro proveeRubro)
        {
            await _context.Provee_Rubro.AddAsync(proveeRubro);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var proveeRubro = await _context.Provee_Rubro.FindAsync(id);
            if (proveeRubro != null)
            {
                _context.Provee_Rubro.Remove(proveeRubro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Provee_Rubro>> GetAllasync()
        {
            return await _context.Provee_Rubro.ToListAsync();
        }

        public async Task<Provee_Rubro> GetAsync(int id)
        {
            return await _context.Provee_Rubro.FindAsync(id);
        }

        public async Task UpdateAsync(Provee_Rubro proveeRubro)
        {
            _context.Provee_Rubro.Update(proveeRubro);
            await _context.SaveChangesAsync();
        }
    }
}
