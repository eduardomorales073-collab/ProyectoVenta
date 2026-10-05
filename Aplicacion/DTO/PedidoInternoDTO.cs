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
    string? nombreDepartamento,
    int? id_OrdenCompra,
    DateTime Fecha_Solicitada,
    DateTime Fecha_Ingreso,
    string? nombreSucursal,
    int? id_Sucursal,
    bool urgente,
    int totalOfertas,          
    bool adjudicado,           
    int? idProveedorGanador,   
    string? nombreProveedorGanador,  
    decimal? precioAdjudicado,
    string? Observaciones,
    List<ArticuloDePedidoDTO> Articulos
);
    public record ArticuloDePedidoDTO(
    int id_Articulo,
    string codigo,
    string nombre,
    int cantidad,
    string? unidadMedida
);
}