

    namespace Aplicacion.DTO
{
    public record LoginDTO(string Email, string Password);
    public record AuthResponseDTO(string Token, string Nombre, string Email, string Rol, int IdRol);
}

