using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Repositorio
{
    public class DetaTelRepositorioImpl : DetaTelRepositorio
    {
        private readonly AplicacionDBContexto _context;

        public DetaTelRepositorioImpl(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task<List<Detalle_Telefono>> GetAllsync()
        {
            return await _context.Detalle_Telefono.ToListAsync();
        }

        public async Task<List<Detalle_Telefono>> GetBySucursalAsync(int idSucursal)
        {
            return await _context.Detalle_Telefono
                .Where(d => d.id_Sucursal == idSucursal)
                .ToListAsync();
        }

        public async Task<Detalle_Telefono> GetAsync(int idSucursal, int idTelefono)
        {
            return await _context.Detalle_Telefono
                .FirstOrDefaultAsync(d => d.id_Sucursal == idSucursal && d.id_Telefono == idTelefono);
        }

        public async Task AddAsync(Detalle_Telefono detalle)
        {
            await _context.Detalle_Telefono.AddAsync(detalle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int idSucursal, int idTelefono)
        {
            var detalle = await GetAsync(idSucursal, idTelefono);
            if (detalle != null)
            {
                _context.Detalle_Telefono.Remove(detalle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBySucursalAsync(int idSucursal)
        {
            var detalles = await GetBySucursalAsync(idSucursal);
            if (detalles.Any())
            {
                _context.Detalle_Telefono.RemoveRange(detalles);
                await _context.SaveChangesAsync();
            }
        }
    }
}