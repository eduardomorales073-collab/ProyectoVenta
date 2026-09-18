using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateRelacionDTO(int Proveedor1, int Proveedor2, string TipoRelacion);
}
