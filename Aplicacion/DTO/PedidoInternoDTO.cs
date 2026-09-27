using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record PedidoInternoDTO(
        int id,
        string? codigo,
        int? cantidad,
        int id_Departamento,
        int? id_OrdenCompra,
        DateTime Fecha_Solicitada,
        DateTime Fecha_Ingreso
    );
}