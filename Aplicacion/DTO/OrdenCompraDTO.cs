using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record OrdenCompraDTO(int id, string Descripcion,DateTime Fecha_Creacion, DateTime Fecha_Limite, int Tipo_Orden);
}
