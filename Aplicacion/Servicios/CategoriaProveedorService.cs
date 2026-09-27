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
    public class CategoriaProveedorService : ICategoriaProveedorService
    {
        private readonly CatProvRepositorio _categoriaRepositorio;
        private readonly IMapper _mapper;

        public CategoriaProveedorService(
            CatProvRepositorio categoriaRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _categoriaRepositorio = categoriaRepositorio;
        }

        public async Task AddAsync(CreateCategoriaProveedorDTO categoria)
        {
            var nuevaCategoria = _mapper.Map<Categoria_Proveedor>(categoria);

            var todos = await _categoriaRepositorio.GetAllasync();
            nuevaCategoria.id = todos.Any() ? todos.Max(c => c.id) + 1 : 1;

            await _categoriaRepositorio.AddAsync(nuevaCategoria);
        }

        public async Task DeleteAsync(int id)
        {
            await _categoriaRepositorio.DeletAsync(id);
        }

        public async Task<List<CategoriaProveedorDTO>> GetAllsync()
        {
            return _mapper.Map<List<CategoriaProveedorDTO>>(await _categoriaRepositorio.GetAllasync());
        }

        public async Task<CategoriaProveedorDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<CategoriaProveedorDTO>(await _categoriaRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateCategoriaProveedorDTO categoria)
        {
            await _categoriaRepositorio.UpdateAsync(_mapper.Map<Categoria_Proveedor>(categoria));
        }
    }
}