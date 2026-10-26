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
        private readonly ProveRubRepositorio _proveRubRepositorio;
        private readonly ProveArtRepositorio _proveArtRepositorio;
        private readonly IMapper _mapper;

        public ProveedorService(
            ProveedorRepositorio proveedorRepository,
            ProveRubRepositorio proveRubRepositorio,
            ProveArtRepositorio proveArtRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _proveedorRepositorio = proveedorRepository;
            _proveRubRepositorio = proveRubRepositorio;
            _proveArtRepositorio = proveArtRepositorio;
        }

        public async Task AddAsync(CreateProveedorDTO proveedor)
        {
            var nuevoProveedor = _mapper.Map<Proveedor>(proveedor);
            var todos = await _proveedorRepositorio.GetAllasync();
            nuevoProveedor.id = todos.Any() ? todos.Max(p => p.id) + 1 : 1;
            await _proveedorRepositorio.AddAsync(nuevoProveedor);

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
            await _proveRubRepositorio.EliminarPorProveedorAsync(id);
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
                p.NIT,
                p.Categoria,
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

            return new ProveedorDTO(
                p.id,
                p.Nombre,
                p.Descripcion,
                p.Telefono,
                p.Direccion,
                p.NIT,
                p.Categoria,
                rubros
            );
        }

        public async Task UpdateAsync(UpdateProveedorDTO proveedor)
        {
            var existente = await _proveedorRepositorio.GetAsync(proveedor.id);
            if (existente == null) throw new System.Exception("Proveedor no encontrado");

            existente.Nombre = proveedor.nombre;
            existente.Descripcion = proveedor.Descripcion;
            existente.Telefono = proveedor.telefono;
            existente.Direccion = proveedor.Direccion;
            existente.NIT = proveedor.NIT;
            existente.Categoria = proveedor.Categoria;

            await _proveedorRepositorio.UpdateAsync(existente);

            await _proveRubRepositorio.EliminarPorProveedorAsync(proveedor.id);

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

        
        public async Task AsignarArticulosAsync(int idProveedor, List<int> idArticulos)
        {
            // 1. Obtener todas las relaciones actuales del proveedor
            var relacionesActuales = await _proveArtRepositorio.GetAllasync();
            var relacionesDelProveedor = relacionesActuales
                .Where(pa => pa.id_Proveedor == idProveedor)
                .ToList();

            // 2. Eliminar las relaciones existentes usando DeletAsync
            foreach (var relacion in relacionesDelProveedor)
            {
                await _proveArtRepositorio.DeletAsync(relacion.id_Proveedor, relacion.id_Articulo);
            }

            // 3. Insertar las nuevas
            foreach (var idArticulo in idArticulos)
            {
                await _proveArtRepositorio.AddAsync(new Provee_Artic
                {
                    id_Proveedor = idProveedor,
                    id_Articulo = idArticulo,
                    Precio = 100.00M //  La 'M' indica decimal
                });
            }
        }
    }
}