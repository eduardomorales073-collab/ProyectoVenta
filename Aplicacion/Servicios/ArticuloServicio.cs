using Aplicacion.DTO;
using Aplicacion.Interfaz;
using AutoMapper;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class ArticuloServicio : IArticuloService
    {
        private readonly ArticuRepositorio _articuloRepository;
        private readonly ProveArtRepositorio _proveeArticRepositorio;
        private readonly ProveedorRepositorio _proveedorRepositorio;
        private readonly ProveRubRepositorio _proveRubRepositorio;
        private readonly IMapper _mapper;

        public ArticuloServicio(
            ArticuRepositorio articuloRepository,
            ProveArtRepositorio proveeArticRepositorio,
            ProveedorRepositorio proveedorRepositorio,
            ProveRubRepositorio proveRubRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _articuloRepository = articuloRepository;
            _proveeArticRepositorio = proveeArticRepositorio;
            _proveedorRepositorio = proveedorRepositorio;
            _proveRubRepositorio = proveRubRepositorio;
        }

        public async Task AddAsync(CreateArticuloDTO articulo)
        {
            var nuevoArticulo = _mapper.Map<Articulo>(articulo);
            var todos = await _articuloRepository.GetAllasync();
            nuevoArticulo.id = todos.Any() ? todos.Max(a => a.id) + 1 : 1;
            await _articuloRepository.AddAsync(nuevoArticulo);

            if (articulo.id_Rubro.HasValue)
            {
                await VincularArticuloConProveedor(nuevoArticulo.id, articulo.id_Rubro.Value);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _articuloRepository.DeletAsync(id);
        }

        public async Task<List<ArticuloDTO>> GetAllsync()
        {
            var articulos = await _articuloRepository.GetAllasync();
            var proveeArtic = await _proveeArticRepositorio.GetAllasync();
            var proveedores = await _proveedorRepositorio.GetAllasync();

            return articulos.Select(a =>
            {
                var relacion = proveeArtic.FirstOrDefault(pa => pa.id_Articulo == a.id);
                string? categoria = null;

                if (relacion != null)
                {
                    var prov = proveedores.FirstOrDefault(p => p.id == relacion.id_Proveedor);
                    categoria = prov?.Categoria;
                }

                return new ArticuloDTO(
                    a.id,
                    a.Nombre,
                    a.Descripcion,
                    a.codigo,
                    a.unidad_medida,
                    categoria ?? "Sin Categoría"
                );
            }).ToList();
        }

        public async Task<ArticuloDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ArticuloDTO>(await _articuloRepository.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateActArtDTO articulo)
        {
            await _articuloRepository.UpdateAsync(_mapper.Map<Articulo>(articulo));

            if (articulo.id_Rubro.HasValue)
            {
                var relaciones = await _proveeArticRepositorio.GetAllasync();
                var relacionExistente = relaciones.FirstOrDefault(pa => pa.id_Articulo == articulo.id);

                if (relacionExistente != null)
                {
                    await _proveeArticRepositorio.DeletAsync(relacionExistente.id_Proveedor, relacionExistente.id_Articulo);
                }

                await VincularArticuloConProveedor(articulo.id, articulo.id_Rubro.Value);
            }
        }

        //  NUEVO MÉTODO: Obtener artículos filtrados por rubro
        public async Task<List<ArticuloDTO>> GetByRubroAsync(int idRubro)
        {
            var articulos = await _articuloRepository.GetAllasync();
            var proveeRubro = await _proveRubRepositorio.GetAllasync();
            var rubros = await _proveRubRepositorio.GetAllasync(); 

            
            string prefijo = "";
            switch (idRubro)
            {
                case 1: prefijo = "TEC-"; break; // Tecnología
                case 2: prefijo = "ALI-"; break; // Alimentos
                case 3: prefijo = "MOB-"; break; // Mobiliario
                case 4: prefijo = "PAP-"; break; // Papelería
                case 5: prefijo = "OTR-"; break; // Otros
                default: return new List<ArticuloDTO>();
            }

            return articulos
                .Where(a => a.codigo != null && a.codigo.StartsWith(prefijo))
                .Select(a => new ArticuloDTO(
                    a.id,
                    a.Nombre,
                    a.Descripcion,
                    a.codigo,
                    a.unidad_medida,
                    prefijo.Replace("-", "") // Nombre del rubro simplificado
                ))
                .ToList();
        }

        private async Task VincularArticuloConProveedor(int idArticulo, int idRubro)
        {
            var proveedoresRubro = await _proveRubRepositorio.GetAllasync();
            var proveedorDelRubro = proveedoresRubro.FirstOrDefault(pr => pr.id_Rubro == idRubro);

            if (proveedorDelRubro != null)
            {
                var nuevaRelacion = new Provee_Artic
                {
                    id_Proveedor = proveedorDelRubro.id_Proveedor,
                    id_Articulo = idArticulo,
                    Precio = 0
                };
                await _proveeArticRepositorio.AddAsync(nuevaRelacion);
            }
        }
    }
}