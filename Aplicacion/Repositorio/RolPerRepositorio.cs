using Aplicacion.modelos;

public interface RolPerRepositorio
{
    Task<List<Rol_Permiso>> GetAllasync();
    Task<Rol_Permiso> GetAsync(int idRol, int idPermiso);
    Task DeletAsync(int idRol, int idPermiso);
    Task AddAsync(Rol_Permiso rolPermiso);
    Task UpdateAsync(Rol_Permiso rolPermiso);
}