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
    public class PermisosService : IPermisosService
    {
        private readonly PermisosRepositorio _permisosRepositorio;
        private readonly IMapper _mapper;
        public PermisosService(PermisosRepositorio permisosRepository, IMapper mapper)
        {
            _mapper = mapper;
            _permisosRepositorio = permisosRepository;

        }
        public async Task AddAsync(CreatePermisosDTO permiso)
        {
            await _permisosRepositorio.AddAsync(_mapper.Map<Permisos>(permiso));
        }

        public async Task DeleteAsync(int id)
        {
            await _permisosRepositorio.DeletAsync(id);
        }

        public async Task<List<PermisosDTO>> GetAllsync()
        {
            return _mapper.Map<List<PermisosDTO>>(await _permisosRepositorio.GetAllasync());
        }

        public async Task<PermisosDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<PermisosDTO>(await _permisosRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdatePermisosDTO permiso)
        {
            await _permisosRepositorio.UpdateAsync(_mapper.Map<Permisos>(permiso));
        }
    }
}
