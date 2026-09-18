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
    public class ProveeArticService : IProvArtService
    {
        private readonly ProveArtRepositorio _proveArtRepositorio;
        private readonly IMapper _mapper;
        public ProveeArticService(ProveArtRepositorio proveArtRepository, IMapper mapper)
        {
            _mapper = mapper;
            _proveArtRepositorio = proveArtRepository;

        }
        public async Task AddAsync(CreateProveeArtcDTO proveeArtc)
        {
            await _proveArtRepositorio.AddAsync(_mapper.Map<Provee_Artic>(proveeArtc));
        }

        public async Task DeleteAsync(int id)
        {
            await _proveArtRepositorio.DeletAsync(id);
        }

        public async Task<List<ProveeArtcDTO>> GetAllsync()
        {
            return _mapper.Map<List<ProveeArtcDTO>>(await _proveArtRepositorio.GetAllasync());
        }

        public async Task<ProveeArtcDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ProveeArtcDTO>(await _proveArtRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateProveeArtcDTO proveeArtc)
        {
            await _proveArtRepositorio.UpdateAsync(_mapper.Map<Provee_Artic>(proveeArtc));
        }
    }
}
