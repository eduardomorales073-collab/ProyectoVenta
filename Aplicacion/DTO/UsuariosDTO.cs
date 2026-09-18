using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UsuariosDTO(int id, string Nombre, string email, string Contraseña,bool Activo,int id_Rol);
}
