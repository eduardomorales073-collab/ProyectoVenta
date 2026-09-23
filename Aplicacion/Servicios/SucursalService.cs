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
    public class SucursalService : ISucursalService
    {
        private readonly SucursalRepositorio _sucursalRepositorio;
        private readonly IMapper _mapper;
        public SucursalService(SucursalRepositorio sucursalRepository, IMapper mapper)
        {
            _mapper = mapper;
            _sucursalRepositorio = sucursalRepository;

        }
        public async Task AddAsync(CreateSucursalDTO sucursal)
        {
            await _sucursalRepositorio.AddAsync(_mapper.Map<Sucursal>(sucursal));
        }

        public async Task DeleteAsync(int id)
        {
            await _sucursalRepositorio.DeletAsync(id);
        }

        public async Task<List<SucursalDTO>> GetAllsync()
        {
            return _mapper.Map<List<SucursalDTO>>(await _sucursalRepositorio.GetAllasync());
        }

        public async Task<SucursalDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<SucursalDTO>(await _sucursalRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateSucursalDTO sucursal)
        {
            await _sucursalRepositorio.UpdateAsync(_mapper.Map<Sucursal>(sucursal)  );
        }
    }
}
