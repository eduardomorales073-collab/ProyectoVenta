using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateOfertaProveedorDTO( int id_Proveedor, int id_Pedido_Interno, decimal Precio, DateTime Fecha_Oferta);

}
