using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class RelacionService : IRelacionService
    {
        private readonly RelacionRepositorio _relacionRepositorio;
        private readonly ProveedorRepositorio _proveedorRepositorio;

        public RelacionService(RelacionRepositorio relacionRepositorio, ProveedorRepositorio proveedorRepositorio)
        {
            _relacionRepositorio = relacionRepositorio;
            _proveedorRepositorio = proveedorRepositorio;
        }

        public async Task<List<RelacionDTO>> GetAllsync()
        {
            var relaciones = await _relacionRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();

            return relaciones.Select(r => new RelacionDTO(
                r.id,
                r.Proveedor1,
                proveedores.FirstOrDefault(p => p.id == r.Proveedor1)?.Nombre ?? "Desconocido",
                r.Proveedor2,
                proveedores.FirstOrDefault(p => p.id == r.Proveedor2)?.Nombre ?? "Desconocido",
                r.TipoRelacion
            )).ToList();
        }

        public async Task AddAsync(CreateRelacionDTO dto)
        {
            var todas = await _relacionRepositorio.GetAllasync();
            var nuevoId = todas.Any() ? todas.Max(r => r.id) + 1 : 1;

            var nuevaRelacion = new Relacion
            {
                id = nuevoId,
                Proveedor1 = dto.Proveedor1,
                Proveedor2 = dto.Proveedor2,
                TipoRelacion = dto.TipoRelacion
            };

            await _relacionRepositorio.AddAsync(nuevaRelacion);
        }

        public async Task DeleteAsync(int id)
        {
            await _relacionRepositorio.DeletAsync(id);
        }
    }
}