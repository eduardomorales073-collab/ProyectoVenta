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
    public class ProveeRubroService : IProvRubService
    {
        private readonly ProveRubRepositorio _proveRubRepositorio;
        private readonly IMapper _mapper;
        public ProveeRubroService(ProveRubRepositorio proveRubRepository, IMapper mapper)
        {
            _mapper = mapper;
            _proveRubRepositorio = proveRubRepository;

        }
        public async Task AddAsync(CreateProveeRubroDTO proveeRubro)
        {
            await _proveRubRepositorio.AddAsync(_mapper.Map<Provee_Rubro>(proveeRubro));
        }

        public async Task DeleteAsync(int id)
        {
            await _proveRubRepositorio.DeletAsync(id);
        }

        public async Task<List<ProveeRubroDTO>> GetAllsync()
        {
            return _mapper.Map<List<ProveeRubroDTO>>(await _proveRubRepositorio.GetAllasync());
        }

        public async Task<ProveeRubroDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<ProveeRubroDTO>(await _proveRubRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateProveeRubroDTO proveeRubro)
        {
            await _proveRubRepositorio.UpdateAsync(_mapper.Map<Provee_Rubro>(proveeRubro));
        }
    }
}
