using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class TiOrRepositorio : TipoOrRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public TiOrRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }
        public async Task AddAsync(Tipo_Orden tipoOrden)
        {
            await _context.Tipo_Orden.AddAsync(tipoOrden);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var tipoOrden = await _context.Tipo_Orden.FindAsync(id);
            if (tipoOrden != null)
            {
                _context.Tipo_Orden.Remove(tipoOrden);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Tipo_Orden>> GetAllasync()
        {
            return await _context.Tipo_Orden.ToListAsync();
        }

        public async Task<Tipo_Orden> GetAsync(int id)
        {
            return await _context.Tipo_Orden.FindAsync(id);
        }

        public async Task UpdateAsync(Tipo_Orden tipoOrden)
        {
            _context.Tipo_Orden.Update(tipoOrden);
            await _context.SaveChangesAsync();
        }
    }
}
