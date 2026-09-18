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
    public class RelacionService : IRelacionService
    {
        private readonly RelacionRepositorio _relacionRepositorio;
        private readonly IMapper _mapper;
        public RelacionService(RelacionRepositorio relacionRepository, IMapper mapper)
        {
            _mapper = mapper;
            _relacionRepositorio = relacionRepository;

        }
        public async Task AddAsync(CreateRelacionDTO relacion)
        {
            await _relacionRepositorio.AddAsync(_mapper.Map<Relacion>(relacion));
        }

        public async Task DeleteAsync(int id)
        {
            await _relacionRepositorio.DeletAsync(id);
        }

        public async Task<List<RelacionDTO>> GetAllsync()
        {
            return _mapper.Map<List<RelacionDTO>>(await _relacionRepositorio.GetAllasync());
        }

        public async  Task<RelacionDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<RelacionDTO>(await _relacionRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateRelacionDTO relacion)
        {
            await _relacionRepositorio.UpdateAsync(_mapper.Map<Relacion>(relacion));
        }
    }
}
