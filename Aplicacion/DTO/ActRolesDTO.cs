using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateRolesDTO(int id, string nombre, string descripcion, bool Externo);

}
