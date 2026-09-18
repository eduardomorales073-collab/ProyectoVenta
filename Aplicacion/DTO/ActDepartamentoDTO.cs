using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateDepartamentoDTO(int id, string nombre, string descripcion, int id_Sucursal);
}
