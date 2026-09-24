using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateUsuariosDTO(string Nombre, string email, string Contrasena, bool Activo, int id_Rol);

}
