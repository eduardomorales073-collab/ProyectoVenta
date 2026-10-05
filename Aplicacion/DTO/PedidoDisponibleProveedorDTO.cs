using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record ProveedorRelacionadoDTO(
        int id,
        string Nombre,
        string TipoRelacion,
        decimal PrecioOfertado
    );

    public record PedidoDisponibleProveedorDTO(
        int id,
        string? codigo,
        int? cantidad,
        int id_Departamento,
        string? nombreDepartamento,
        int? id_OrdenCompra,
        DateTime Fecha_Solicitada,
        DateTime Fecha_Ingreso,
        string? nombreSucursal,
        int? id_Sucursal,
        bool urgente,
        string? Observaciones,
        int totalOfertas,
        bool adjudicado,
        List<ArticuloDePedidoDTO> Articulos,
        // ✅ Info de competencia
        int OfertasDeMiRubro,
        decimal? PrecioMinimoDelRubro,
        bool YaOferte,
        // ✅ NUEVOS
        decimal? MiPrecioOfertado,
        List<ProveedorRelacionadoDTO> ProveedoresRelacionadosQueOfertaron
    );
}