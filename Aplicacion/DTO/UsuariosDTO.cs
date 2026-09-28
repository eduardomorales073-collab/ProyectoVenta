using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UsuariosDTO(
        int id,
        string Nombre,
        string email,
        string Contrasena,
        bool Activo,
        int id_Rol,
        int? id_Proveedor,
        int? id_Departamento
    );
}