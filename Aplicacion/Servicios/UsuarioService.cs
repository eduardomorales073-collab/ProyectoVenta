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
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        public UsuarioService(UsuarioRepositorio usuarioRepository, IMapper mapper)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepository;

        }
        public async Task AddAsync(CreateUsuariosDTO usuario)
        {
            await _usuarioRepositorio.AddAsync(_mapper.Map<Usuarios>(usuario));
        }

        public async Task DeleteAsync(int id)
        {
            await _usuarioRepositorio.DeletAsync(id);
        }

        public async Task<List<UsuariosDTO>> GetAllsync()
        {
            return _mapper.Map<List<UsuariosDTO>>(await _usuarioRepositorio.GetAllasync());
        }

        public async Task<UsuariosDTO> GetByIdAsync(int id)
        {
            return _mapper.Map<UsuariosDTO>(await _usuarioRepositorio.GetAsync(id));
        }

        public async Task UpdateAsync(UpdateUsuariosDTO usuario)
        {
            await _usuarioRepositorio.UpdateAsync(_mapper.Map<Usuarios>(usuario));
        }
        
    }
}
