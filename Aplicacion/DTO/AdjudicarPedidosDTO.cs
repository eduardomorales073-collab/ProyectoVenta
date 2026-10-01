using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    /// <summary>
    /// DTO para adjudicar múltiples pedidos de una orden en un solo paso.
    /// </summary>
    public record AdjudicarPedidosDTO(
        int Orden_Compra,
        List<PedidoAdjudicarDTO> Pedidos
    );

    public record PedidoAdjudicarDTO(
        int id_Pedido,
        int id_Proveedor
    );
}