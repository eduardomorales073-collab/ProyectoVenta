using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    internal class AplicacionDTO
    {
        public record LoginDTO(string Email, string Password);
        public record AuthResponseDTO(string Token, string Nombre, string Email);
    }
}
