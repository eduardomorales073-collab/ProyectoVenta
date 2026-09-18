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
    public class DireccionService : IDireccionService
    {
        private readonly DireccionRepositorio _direccionRepositorio;
        private readonly IMapper _mapper;
        public DireccionService(DireccionRepositorio direccionRepository, IMapper mapper)
        {
            _mapper = mapper;
            _direccionRepositorio = direccionRepository;

        }
        public async Task AddAsync(CreateDireccionDTO direccion)
        {
            await _direccionRepositorio.AddAsync(_mapper.Map<Direccion>(direccion));
        }

        public async Task DeleteAsync(int id)
        {
            await _direccionRepositorio.DeletAsync(id);
        }

        public async Task<List<DireccionDTO>> GetAllsync()
        {
            return _mapper.Map<List<DireccionDTO>>(await _direccionRepositorio.GetAllasync());
            
        }

        public async Task<DireccionDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<DireccionDTO>(await _direccionRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateDireccionDTO direccion)
        {
            await _direccionRepositorio.UpdateAsync(_mapper.Map<Direccion>(direccion));
        }
    }
}
