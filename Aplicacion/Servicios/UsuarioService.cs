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
            var usuario = await _usuarioRepositorio.GetAsync(idUsuario);
            if (usuario == null) return null;

            var permisos = await _usuarioRepositorio.ObtenerPermisosDelUsuarioAsync(idUsuario);
            if (permisos == null) return null;

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

        public async Task<bool> ActualizarPermisosAsync(int idUsuario, UpdatePermisosDTO dto)
        {
            return await _usuarioRepositorio.ActualizarPermisosAsync(
                dto.id,
                dto.Crear,
                dto.Leer,
                dto.Actualizar,
                dto.Borrar,
                dto.fecha
            );
        }

        public async Task<PerfilDTO?> ObtenerPerfilAsync(int id)
        {
            var usuario = await _usuarioRepositorio.GetAsync(id);
            if (usuario == null) return null;

            return new PerfilDTO(
                usuario.id,
                usuario.Nombre,
                usuario.email,
                usuario.Activo,
                usuario.id_Rol
            );
        }

        public async Task<(bool ok, string mensaje)> ActualizarPerfilAsync(int id, UpdatePerfilDTO dto)
        {
            var usuario = await _usuarioRepositorio.GetAsync(id);
            if (usuario == null) return (false, "Usuario no encontrado.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return (false, "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return (false, "El correo es obligatorio.");

            // Validar email único
            if (await _usuarioRepositorio.ExisteEmailAsync(dto.Email, id))
                return (false, "El correo ya está en uso por otro usuario.");

            usuario.Nombre = dto.Nombre.Trim();
            usuario.email = dto.Email.Trim();

            await _usuarioRepositorio.UpdateAsync(usuario);
            return (true, "Perfil actualizado correctamente.");
        }

        public async Task<(bool ok, string mensaje)> CambiarContrasenaAsync(int id, ChangePasswordDTO dto)
        {
            var usuario = await _usuarioRepositorio.GetAsync(id);
            if (usuario == null) return (false, "Usuario no encontrado.");

            if (string.IsNullOrWhiteSpace(dto.ContrasenaActual) ||
                string.IsNullOrWhiteSpace(dto.ContrasenaNueva) ||
                string.IsNullOrWhiteSpace(dto.ConfirmarContrasena))
                return (false, "Todos los campos son obligatorios.");

            if (dto.ContrasenaNueva != dto.ConfirmarContrasena)
                return (false, "Las contraseñas nuevas no coinciden.");

            if (dto.ContrasenaNueva.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres.");

            // Verificar contraseña actual con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.ContrasenaActual, usuario.Contrasena))
                return (false, "La contraseña actual es incorrecta.");

            if (dto.ContrasenaActual == dto.ContrasenaNueva)
                return (false, "La nueva contraseña debe ser diferente a la actual.");

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.ContrasenaNueva);
            await _usuarioRepositorio.UpdateAsync(usuario);

            return (true, "Contraseña actualizada correctamente.");
        }
    }
}