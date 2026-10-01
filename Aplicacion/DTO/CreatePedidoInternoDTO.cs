using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreatePedidoInternoDTO(
        string? codigo,
        int? cantidad,
        int id_Departamento,
        int? id_OrdenCompra,
        DateTime Fecha_Solicitada,
        bool urgente                   
    );
}