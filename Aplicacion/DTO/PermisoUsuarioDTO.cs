namespace Aplicacion.DTO
{
    public record PermisoUsuarioDTO(
        int IdUsuario,
        string NombreUsuario,
        int IdRol,
        int IdPermiso,
        bool Crear,
        bool Leer,
        bool Actualizar,
        bool Borrar
    );
}