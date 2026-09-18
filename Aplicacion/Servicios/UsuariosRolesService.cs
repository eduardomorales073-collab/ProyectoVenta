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
    internal class UsuariosRolesService : IUsuarioRolesService
    {
        private readonly UsRolRepositorio _usrolRepositorio;
        private readonly IMapper _mapper;
        public UsuariosRolesService(UsRolRepositorio usrolRepository, IMapper mapper)
        {
            _mapper = mapper;
            _usrolRepositorio = usrolRepository;

        }
        public async Task AddAsync(CreateUsuariosRolesDTO usuarioRole)
        {
            await _usrolRepositorio.AddAsync(_mapper.Map<Usuarios_Roles>(usuarioRole));
        }

        public async Task DeleteAsync(int id)
        {
            await _usrolRepositorio.DeletAsync(id);
        }

        public async Task<List<UsuariosRolesDTO>> GetAllsync()
        {
            return _mapper.Map<List<UsuariosRolesDTO>>(await _usrolRepositorio.GetAllasync());
        }

        public async Task<UsuariosRolesDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<UsuariosRolesDTO>(await _usrolRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateUsuariosRolesDTO usuarioRole)
        {
            await _usrolRepositorio.UpdateAsync(_mapper.Map<Usuarios_Roles>(usuarioRole));
        }
    }
}
