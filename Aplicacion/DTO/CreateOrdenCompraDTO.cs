using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateOrdenCompraDTO(
        string Descripcion,
        DateTime Fecha_Creacion,
        DateTime Fecha_Limite,
        DateTime? fecha_limite_ofertas,
        int Tipo_Orden
    );
}