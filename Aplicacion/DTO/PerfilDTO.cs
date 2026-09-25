namespace Aplicacion.DTO
{
    public record PerfilDTO(int id, string Nombre, string Email, bool Activo, int id_Rol);

    public record UpdatePerfilDTO(string Nombre, string Email);

    public record ChangePasswordDTO(
        string ContrasenaActual,
        string ContrasenaNueva,
        string ConfirmarContrasena
    );
}