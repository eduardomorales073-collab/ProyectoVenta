using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record PedidoInternoDTO(int id, int id_Departamento, DateTime Fecha_Solicitada, DateTime Fecha_Ingreso);
}
