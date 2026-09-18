using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Servicios
{
    internal class RubroService : IRubroService
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
            await _rubroRepositorio.AddAsync(_mapper.Map<Rubro>(rubro));
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
