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
    public class AdjudicacionService : IAdjudicacionService
    {
        private readonly AdjuRepositorio _adjuRepositorio;
        private readonly IMapper _mapper;
        public AdjudicacionService(AdjuRepositorio adjuRepository, IMapper mapper)
        {
            _mapper = mapper;
            _adjuRepositorio = adjuRepository;

        }
        public async Task AddAsync(CreateAdjudicacionDTO adjudicacion)
        {
            await _adjuRepositorio.AddAsync(_mapper.Map<Adjudicacion>(adjudicacion));
        }

        public async Task DeleteAsync(int id)
        {
            await _adjuRepositorio.DeletAsync(id);
        }

        public async Task<List<AdjudicacionDTO>> GetAllsync()
        {
            return _mapper.Map<List<AdjudicacionDTO>>(await _adjuRepositorio.GetAllasync());
        }

        public async Task<AdjudicacionDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<AdjudicacionDTO>(await _adjuRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateAdjudicacionDTO adjudicacion)
        {
            await _adjuRepositorio.UpdateAsync(_mapper.Map<Adjudicacion>(adjudicacion));
        }
    }
}
