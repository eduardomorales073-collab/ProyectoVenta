using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record DetalleAdjudicacionDTO(int id_Adjudicacion, int  id_Pedido, int id_Proveedor, decimal Precio, int Cantidad);
}
