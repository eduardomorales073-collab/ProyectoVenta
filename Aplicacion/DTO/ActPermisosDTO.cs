using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdatePermisosDTO(int id, bool Crear, bool Leer, bool Actualizar, bool Borrar, DateTime fecha);

}
