using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateAdjudicacionDTO(int id, DateTime Fecha_Resolucion, int Orden_Compra, string Estado);
}
