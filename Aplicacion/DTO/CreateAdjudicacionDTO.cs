using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateAdjudicacionDTO(DateTime Fecha, int Orden_Compra, string Estado);
}
