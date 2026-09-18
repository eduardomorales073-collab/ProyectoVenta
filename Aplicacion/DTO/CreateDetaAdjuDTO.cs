using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateDetalleAdjudicacionDTO(int id_Adjudicacion, int id_Pedido, int id_Proveedor, decimal Precio, int Cantidad);
}
