using Aplicacion.modelos;
using Aplicacion.Repositorio;
using Infraestructura.Datos;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositorio
{
    public class ProvRubRepositorio : ProveRubRepositorio
    {
        private readonly AplicacionDBContexto _context;

        public ProvRubRepositorio(AplicacionDBContexto context)
        {
            _context = context;
        }

        public async Task<List<Provee_Rubro>> GetAllasync()
        {
            return await _context.Provee_Rubro.ToListAsync();
        }

        // ✅ Búsqueda por clave compuesta
        public async Task<Provee_Rubro> GetAsync(int idProveedor, int idRubro)
        {
            return await _context.Provee_Rubro
                .FirstOrDefaultAsync(pr => pr.id_Proveedor == idProveedor && pr.id_Rubro == idRubro);
        }

        // ✅ Eliminar por clave compuesta
        public async Task DeletAsync(int idProveedor, int idRubro)
        {
            var proveeRubro = await _context.Provee_Rubro
                .FirstOrDefaultAsync(pr => pr.id_Proveedor == idProveedor && pr.id_Rubro == idRubro);

            if (proveeRubro != null)
            {
                _context.Provee_Rubro.Remove(proveeRubro);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddAsync(Provee_Rubro proveeRubro)
        {
            await _context.Provee_Rubro.AddAsync(proveeRubro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Provee_Rubro proveeRubro)
        {
            _context.Provee_Rubro.Update(proveeRubro);
            await _context.SaveChangesAsync();
        }

        // ✅ NUEVO: Eliminar todas las asociaciones de un proveedor
        public async Task EliminarPorProveedorAsync(int idProveedor)
        {
            var asociaciones = await _context.Provee_Rubro
                .Where(pr => pr.id_Proveedor == idProveedor)
                .ToListAsync();

            if (asociaciones.Any())
            {
                _context.Provee_Rubro.RemoveRange(asociaciones);
                await _context.SaveChangesAsync();
            }
        }
    }
}