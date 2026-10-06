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

        // ✅ Clave por defecto si no está configurada (32+ caracteres)
        private const string DEFAULT_JWT_KEY = "una-clave-secreta-muy-larga-de-al-menos-32-caracteres-para-desarrollo";

        public AuthService(UsuarioRepositorio usuarioRepositorio, IConfiguration config)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _config = config;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO dto)
        {
            // ✅ Validar que email y password no sean null
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return null;

            // 1. Buscar el usuario
            var usuarios = await _usuarioRepositorio.GetAllasync();
            var usuario = usuarios.FirstOrDefault(u =>
                !string.IsNullOrEmpty(u.email) &&
                u.email.ToLower() == dto.Email.ToLower() &&
                u.Activo);
            if (usuario == null)
                return null;

            // 2. Verificar la contraseña
            if (string.IsNullOrEmpty(usuario.Contrasena))
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.Contrasena))
                return null;

            // 3. Generar el token JWT
            var token = GenerarToken(usuario);

            // 4. Devolver respuesta
            return new AuthResponseDTO(
                token,
                usuario.Nombre,
                usuario.email,
                usuario.id_Rol.ToString(),
                usuario.id_Rol
            );
        }

        private string GenerarToken(modelos.Usuarios usuario)
        {
            string nombreRol = usuario.id_Rol switch
            {
                1 => "Administrador",
                2 => "GestorCompras",
                3 => "AdministradorProveedor",
                4 => "Auditor",
                5 => "CreadorPedidos",
                _ => "Auditor"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                new Claim(ClaimTypes.Email, usuario.email ?? ""),
                new Claim(ClaimTypes.Name, usuario.Nombre ?? ""),
                new Claim(ClaimTypes.Role, nombreRol),
                new Claim("IdRol", usuario.id_Rol.ToString())
            };

            // IdProveedor (si existe)
            if (usuario.id_Proveedor.HasValue)
            {
                claims.Add(new Claim("IdProveedor", usuario.id_Proveedor.Value.ToString()));
            }

            // IdDepartamento (si existe)
            if (usuario.id_Departamento.HasValue)
            {
                claims.Add(new Claim("IdDepartamento", usuario.id_Departamento.Value.ToString()));
            }

            // ✅ Leer la clave JWT con FALLBACK
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                Console.WriteLine("!!! ADVERTENCIA: Jwt:Key es null o vacío. Usando clave por defecto.");
                jwtKey = DEFAULT_JWT_KEY;
            }

            // ✅ Log de diagnóstico
            Console.WriteLine($"[AuthService] Jwt:Key (primeros 20 chars): {jwtKey.Substring(0, Math.Min(20, jwtKey.Length))}...");
            Console.WriteLine($"[AuthService] Jwt:Key length: {jwtKey.Length}");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var issuer = _config["Jwt:Issuer"] ?? "ProyectoVentas";
            var audience = _config["Jwt:Audience"] ?? "ProyectoVentasUsuarios";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}