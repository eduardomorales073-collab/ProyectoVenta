using Aplicacion.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Interfaz
{
    public interface IUsuarioService
    {
        Task<List<UsuariosDTO>> GetAllsync();
        Task<UsuariosDTO> GetByIdAsync(int id);
        Task AddAsync(CreateUsuariosDTO usuario);
        Task DeleteAsync(int id);
        Task UpdateAsync(UpdateUsuariosDTO usuario);
        Task<PermisoUsuarioDTO?> ObtenerPermisosAsync(int idUsuario);
        Task<bool> ActualizarPermisosAsync(int idUsuario, UpdatePermisosDTO dto);

        // ← NUEVOS
        Task<PerfilDTO?> ObtenerPerfilAsync(int id);
        Task<(bool ok, string mensaje)> ActualizarPerfilAsync(int id, UpdatePerfilDTO dto);
        Task<(bool ok, string mensaje)> CambiarContrasenaAsync(int id, ChangePasswordDTO dto);
    }
}
