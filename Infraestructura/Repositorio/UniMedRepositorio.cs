using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class UniMedRepositorio : UnidadMedidaRepositorio
    {
        private readonly AplicacionDBContexto _context;

        public UniMedRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Unidad_Medida unidad)
        {
            await _context.Unidad_Medida.AddAsync(unidad);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int id)
        {
            var unidad = await _context.Unidad_Medida.FindAsync(id);
            if (unidad != null)
            {
                _context.Unidad_Medida.Remove(unidad);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Unidad_Medida>> GetAllasync()
        {
            return await _context.Unidad_Medida.ToListAsync();
        }

        public async Task<Unidad_Medida> GetAsync(int id)
        {
            return await _context.Unidad_Medida.FindAsync(id);
        }

        public async Task UpdateAsync(Unidad_Medida unidad)
        {
            _context.Unidad_Medida.Update(unidad);
            await _context.SaveChangesAsync();
        }
    }
}