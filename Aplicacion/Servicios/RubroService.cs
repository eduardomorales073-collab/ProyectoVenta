using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Aplicacion.Servicios
{
    public class RubroService : IRubroService
    {

        private readonly RubroRepositorio _rubroRepositorio;
        private readonly IMapper _mapper;
        public RubroService(RubroRepositorio rubroRepository, IMapper mapper)
        {
            _mapper = mapper;
            _rubroRepositorio = rubroRepository;

        }
        public async Task AddAsync(CreateRubroDTO rubro)
        {
            // 1. Mapear el DTO a la entidad
            var nuevoRubro = _mapper.Map<Rubro>(rubro);

            // 2. Calcular el siguiente id
            var todos = await _rubroRepositorio.GetAllasync();
            nuevoRubro.id = todos.Any() ? todos.Max(r => r.id) + 1 : 1;

            // 3. Guardar
            await _rubroRepositorio.AddAsync(nuevoRubro);
        }

        public async Task DeleteAsync(int id)
        {
            await _rubroRepositorio.DeletAsync(id);
        }

        public async Task<List<RubroDTO>> GetAllsync()
        {
            return _mapper.Map<List<RubroDTO>>(await _rubroRepositorio.GetAllasync());
        }

        public async Task<RubroDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<RubroDTO>(await _rubroRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateRubroDTO rubro)
        {
            await _rubroRepositorio.UpdateAsync(_mapper.Map<Rubro>(rubro));
        }
    }
}
