using Aplicacion.DTO;
using Aplicacion.Repositorio;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aplicacion.Servicios
{
    public class AuthService
    {
        private readonly UsuarioRepositorio _usuarioRepositorio;
        private readonly IConfiguration _config;

        public AuthService(UsuarioRepositorio usuarioRepositorio, IConfiguration config)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _config = config;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var usuarios = await _usuarioRepositorio.GetAllasync();
            var usuario = usuarios.FirstOrDefault(u => u.email == dto.Email && u.Activo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Contrasena))
                return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                new Claim(ClaimTypes.Email, usuario.email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.id_Rol.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return new AuthResponseDTO(new JwtSecurityTokenHandler().WriteToken(token), usuario.Nombre, usuario.email);
        }
    }
}