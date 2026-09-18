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
    internal class RolPermisoService : IRolPermiService
    {
        private readonly RolPerRepositorio _rolPerRepositorio;
        private readonly IMapper _mapper;
        public RolPermisoService(RolPerRepositorio rolPerRepository, IMapper mapper)
        {
            _mapper = mapper;
            _rolPerRepositorio = rolPerRepository;

        }
        public async Task AddAsync(CreateRolPermisoDTO rolPermiso)
        {
            await _rolPerRepositorio.AddAsync(_mapper.Map<Rol_Permiso>(rolPermiso));
        }

        public async Task DeleteAsync(int id)
        {
            await _rolPerRepositorio.DeletAsync(id);
        }

        public async Task<List<RolPermisoDTO>> GetAllsync()
        {
            return _mapper.Map<List<RolPermisoDTO>>(await _rolPerRepositorio.GetAllasync());
        }

        public async Task<RolPermisoDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<RolPermisoDTO>(await _rolPerRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateRolPermisoDTO rolPermiso)
        {
            await _rolPerRepositorio.UpdateAsync(_mapper.Map<Rol_Permiso>(rolPermiso));
        }
    }
}
