using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class ProveedorService : IProveedorService
    {
        private readonly ProveedorRepositorio _proveedorRepositorio;
        private readonly ProveRubRepositorio _proveRubRepositorio;   // ← NUEVO
        private readonly IMapper _mapper;

        public ProveedorService(
            ProveedorRepositorio proveedorRepository,
            ProveRubRepositorio proveRubRepositorio,                  // ← NUEVO
            IMapper mapper)
        {
            _mapper = mapper;
            _proveedorRepositorio = proveedorRepository;
            _proveRubRepositorio = proveRubRepositorio;               // ← NUEVO
        }

        public async Task AddAsync(CreateProveedorDTO proveedor)
        {
            // 1. Mapear
            var nuevoProveedor = _mapper.Map<Proveedor>(proveedor);

            // 2. Calcular el id
            var todos = await _proveedorRepositorio.GetAllasync();
            nuevoProveedor.id = todos.Any() ? todos.Max(p => p.id) + 1 : 1;

            // 3. Guardar el proveedor
            await _proveedorRepositorio.AddAsync(nuevoProveedor);

            // 4. Guardar las asociaciones con rubros
            if (proveedor.id_Rubros != null)
            {
                foreach (var idRubro in proveedor.id_Rubros)
                {
                    await _proveRubRepositorio.AddAsync(new Provee_Rubro
                    {
                        id_Proveedor = nuevoProveedor.id,
                        id_Rubro = idRubro
                    });
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            // 1. Eliminar asociaciones primero
            await _proveRubRepositorio.EliminarPorProveedorAsync(id);

            // 2. Eliminar el proveedor
            await _proveedorRepositorio.DeletAsync(id);
        }

        public async Task<List<ProveedorDTO>> GetAllsync()
        {
            var proveedores = await _proveedorRepositorio.GetAllasync();
            var asociaciones = await _proveRubRepositorio.GetAllasync();

            return proveedores.Select(p => new ProveedorDTO(
                p.id,
                p.Nombre,
                p.Descripcion,
                p.Telefono,
                p.Direccion,
                asociaciones.Where(a => a.id_Proveedor == p.id).Select(a => a.id_Rubro).ToList()
            )).ToList();
        }

        public async Task<ProveedorDTO> GetByIdAsync(int id)
        {
            var p = await _proveedorRepositorio.GetAsync(id);
            if (p == null) return null;

            var asociaciones = await _proveRubRepositorio.GetAllasync();
            var rubros = asociaciones
                .Where(a => a.id_Proveedor == id)
                .Select(a => a.id_Rubro)
                .ToList();

            return new ProveedorDTO(p.id, p.Nombre, p.Descripcion, p.Telefono, p.Direccion, rubros);
        }

        public async Task UpdateAsync(UpdateProveedorDTO proveedor)
        {
            // 1. Actualizar el proveedor
            var existente = await _proveedorRepositorio.GetAsync(proveedor.id);
            if (existente == null) throw new System.Exception("Proveedor no encontrado");

            existente.Nombre = proveedor.nombre;
            existente.Descripcion = proveedor.Descripcion;
            existente.Telefono = proveedor.telefono;
            existente.Direccion = proveedor.Direccion;

            await _proveedorRepositorio.UpdateAsync(existente);

            // 2. Eliminar asociaciones anteriores
            await _proveRubRepositorio.EliminarPorProveedorAsync(proveedor.id);

            // 3. Crear nuevas asociaciones
            if (proveedor.id_Rubros != null)
            {
                foreach (var idRubro in proveedor.id_Rubros)
                {
                    await _proveRubRepositorio.AddAsync(new Provee_Rubro
                    {
                        id_Proveedor = proveedor.id,
                        id_Rubro = idRubro
                    });
                }
            }
        }
    }
}