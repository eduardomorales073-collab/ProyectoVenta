using Aplicacion.DTO;

namespace Aplicacion.Interfaz
{
    public interface IUsuarioRolesService
    {
        Task<List<UsuariosRolesDTO>> GetAllsync();
        Task<UsuariosRolesDTO> GetByIdAsync(int idUsuario, int idRol);
        Task AddAsync(CreateUsuariosRolesDTO dto);
        Task DeleteAsync(int idUsuario, int idRol);
        Task UpdateAsync(UpdateUsuariosRolesDTO dto);
    }
}