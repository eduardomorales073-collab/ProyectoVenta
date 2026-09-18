using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateProveeArtcDTO(int id_Proveedor, int id_Articulo, Decimal Precio);

}
