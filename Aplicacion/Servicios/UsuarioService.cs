using Aplicacion.DTO;
using Aplicacion.Interfaz;
using Aplicacion.modelos;
using Aplicacion.Repositorio;
using AutoMapper;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var nuevoUsuario = _mapper.Map<Usuarios>(usuario);
            nuevoUsuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            var todos = await _usuarioRepositorio.GetAllasync();
            nuevoUsuario.id = todos.Any() ? todos.Max(u => u.id) + 1 : 1;

            await _usuarioRepositorio.AddAsync(nuevoUsuario);
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
            var usuarioExistente = await _usuarioRepositorio.GetAsync(usuario.id);
            if (usuarioExistente == null)
                throw new Exception("Usuario no encontrado");

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.email = usuario.email;
            usuarioExistente.Activo = usuario.Activo;
            usuarioExistente.id_Rol = usuario.id_Rol;

            if (!string.IsNullOrWhiteSpace(usuario.Contrasena))
            {
                usuarioExistente.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            }

            await _usuarioRepositorio.UpdateAsync(usuarioExistente);
        }

        public async Task<PermisoUsuarioDTO?> ObtenerPermisosAsync(int idUsuario)
        {
            // 1. Obtener el usuario
            var usuario = await _usuarioRepositorio.GetAsync(idUsuario);
            if (usuario == null) return null;

            // 2. Obtener los permisos del rol del usuario
            var permisos = await _usuarioRepositorio.ObtenerPermisosDelUsuarioAsync(idUsuario);
            if (permisos == null) return null;

            // 3. Construir la respuesta
            return new PermisoUsuarioDTO(
                usuario.id,
                usuario.Nombre,
                usuario.id_Rol,
                permisos.id,
                permisos.Crear,
                permisos.Leer,
                permisos.Actualizar,
                permisos.Borrar
            );
        }
    }
}