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
    public class UnidadMedidaService : IUnidadMedidaService
    {
        private readonly UnidadMedidaRepositorio _unidadRepositorio;
        private readonly IMapper _mapper;

        public UnidadMedidaService(
            UnidadMedidaRepositorio unidadRepositorio,
            IMapper mapper)
        {
            _mapper = mapper;
            _unidadRepositorio = unidadRepositorio;
        }

        public async Task AddAsync(CreateUnidadMedidaDTO unidad)
        {
            var nuevaUnidad = _mapper.Map<Unidad_Medida>(unidad);

            var todos = await _unidadRepositorio.GetAllasync();
            nuevaUnidad.id = todos.Any() ? todos.Max(u => u.id) + 1 : 1;

            await _unidadRepositorio.AddAsync(nuevaUnidad);
        }

        public async Task DeleteAsync(int id)
        {
            await _unidadRepositorio.DeletAsync(id);
        }

        public async Task<List<UnidadMedidaDTO>> GetAllsync()
        {
            return _mapper.Map<List<UnidadMedidaDTO>>(await _unidadRepositorio.GetAllasync());
        }

        public async Task<UnidadMedidaDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<UnidadMedidaDTO>(await _unidadRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateUnidadMedidaDTO unidad)
        {
            await _unidadRepositorio.UpdateAsync(_mapper.Map<Unidad_Medida>(unidad));
        }
    }
}