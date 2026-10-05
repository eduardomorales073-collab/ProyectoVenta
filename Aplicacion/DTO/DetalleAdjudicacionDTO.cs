using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record ProveedorRelacionInfoDTO(
        int id,
        string Nombre,
        string TipoRelacion
    );

    public record DetalleAdjudicacionDTO(
        int id_Adjudicacion,
        int id_Pedido,
        int id_Proveedor,
        decimal Precio,
        int Cantidad,
        // ✅ NUEVOS
        string? NombreProveedor,
        List<ProveedorRelacionInfoDTO> RelacionesProveedor
    );
}