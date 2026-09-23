using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record AdjudicacionDTO(int id, DateTime Fecha_Resolucion, int Orden_Compra, string Estado);
}
