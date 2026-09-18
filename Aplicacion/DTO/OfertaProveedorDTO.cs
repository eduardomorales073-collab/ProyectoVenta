using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record OfertaProveedorDTO(int id, int id_Proveedor, int id_Pedido_Interno, decimal Precio, DateTime Fecha_Oferta);
}
