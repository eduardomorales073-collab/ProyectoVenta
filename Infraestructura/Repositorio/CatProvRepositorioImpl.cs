using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class CatProvRepositorioImpl : CatProvRepositorio
    {
        private readonly AplicacionDBContexto _context;

        public CatProvRepositorioImpl(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Categoria_Proveedor categoria)
        {
            await _context.Categoria_Proveedor.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var categoria = await _context.Categoria_Proveedor.FindAsync(id);
            if (categoria != null)
            {
                _context.Categoria_Proveedor.Remove(categoria);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Categoria_Proveedor>> GetAllasync()
        {
            return await _context.Categoria_Proveedor.ToListAsync();
        }

        public async Task<Categoria_Proveedor> GetAsync(int id)
        {
            return await _context.Categoria_Proveedor.FindAsync(id);
        }

        public async Task UpdateAsync(Categoria_Proveedor categoria)
        {
            _context.Categoria_Proveedor.Update(categoria);
            await _context.SaveChangesAsync();
        }
    }
}