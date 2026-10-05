using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record OrdenCompraConContadoresDTO(
     int id,
     string Descripcion,
     DateTime Fecha_Creacion,
     DateTime Fecha_Limite,
     DateTime? fecha_limite_ofertas,
     int Tipo_Orden,
     int TotalPedidos,
     int PedidosAdjudicados,
     int PedidosPendientes,
     string Estado,
     int? id_UsuarioCreador,
     string? NombreSucursal,         
     string? NombreDepartamento       
 );
}