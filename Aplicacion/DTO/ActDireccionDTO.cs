using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateDireccionDTO(int id, int id_Sucursal, string Ciudad, string Estado);
}
