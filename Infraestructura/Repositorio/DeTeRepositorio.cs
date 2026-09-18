using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Repositorio
{
    public class DeTeRepositorio : DetTelRepositorio
    {
        private readonly AplicacionDBContexto _context;
        public DeTeRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task AddAsync(Detalle_Telefono detalleTelefono)
        {
            await _context.Detalle_Telefono.AddAsync(detalleTelefono);
            await _context.SaveChangesAsync();
        }

        public async Task DeletAsync(int idSucursal, int idTelefono)
        {
            var detalleTelefono = await _context.Detalle_Telefono
                .FindAsync(idSucursal, idTelefono);
            if (detalleTelefono != null)
            {
                _context.Detalle_Telefono.Remove(detalleTelefono);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Detalle_Telefono>> GetAllasync()
        {
            return await _context.Detalle_Telefono.ToListAsync();
        }

        public async Task<Detalle_Telefono> GetAsync(int idSucursal, int idTelefono)
        {
            var detalleTelefono = await _context.Detalle_Telefono
                .FindAsync(idSucursal, idTelefono);
            return detalleTelefono!;
        }

        public async Task UpdateAsync(Detalle_Telefono detalleTelefono)
        {
            _context.Detalle_Telefono.Update(detalleTelefono);
        }

    }
}