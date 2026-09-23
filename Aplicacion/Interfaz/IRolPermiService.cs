using Aplicacion.DTO;

namespace Aplicacion.Interfaz
{
    public interface IRolPermiService
    {
        Task<List<RolPermisoDTO>> GetAllsync();
        Task<RolPermisoDTO> GetByIdAsync(int idRol, int idPermiso);
        Task AddAsync(CreateRolPermisoDTO dto);
        Task DeleteAsync(int idRol, int idPermiso);
        Task UpdateAsync(UpdateRolPermisoDTO dto);
    }
}