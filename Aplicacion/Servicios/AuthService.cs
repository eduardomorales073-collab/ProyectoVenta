using Aplicacion.DTO;
using Aplicacion.Repositorio;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            // 1. Buscar el usuario
            var usuarios = await _usuarioRepositorio.GetAllasync();
            var usuario = usuarios.FirstOrDefault(u => u.email == dto.Email && u.Activo);

            if (usuario == null)
                return null;

            // 2. Verificar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Contrasena))
                return null;

            // 3. Generar el token JWT
            var token = GenerarToken(usuario);

            // 4. Devolver respuesta
            return new AuthResponseDTO(
                token,
                usuario.Nombre,
                usuario.email,
                usuario.id_Rol.ToString(),   // ← String del id del rol
                usuario.id_Rol                // ← int del id del rol
            );
        }

        private string GenerarToken(modelos.Usuarios usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                new Claim(ClaimTypes.Email, usuario.email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim("IdRol", usuario.id_Rol.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}