using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infraestructura.Repositorio
{
    public class ArticuloRepositorio : ArticuRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public ArticuloRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task<List<Articulo>> GetAllasync()
        {
            return await _context.Articulo.ToListAsync();
        }

        public async Task<Articulo> GetAsync(int id)
        {
            var articulo = await _context.Articulo.FindAsync(id);
            return articulo!;
        }

        public async Task DeletAsync(int id)
        {
            var articulo = await _context.Articulo.FindAsync(id);
            if (articulo != null)
            {
                _context.Articulo.Remove(articulo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddAsync(Articulo articulo)
        {
            await _context.Articulo.AddAsync(articulo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Articulo articulo)
        {
            _context.Articulo.Update(articulo);
            await _context.SaveChangesAsync();
        }
    }
}
