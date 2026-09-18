using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record PermisosDTO(int id, bool Crear, bool Leer, bool Actualizar, bool Borrar, DateTime fecha);
}
