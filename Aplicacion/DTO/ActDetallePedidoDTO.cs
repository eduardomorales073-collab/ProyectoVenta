using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateDetallePedidoDTO(int id_Pedido, int id_Articulo, int Cantidad);
}
