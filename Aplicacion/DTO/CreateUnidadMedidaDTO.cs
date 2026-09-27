using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateUnidadMedidaDTO(
        string Nombre,
        string Abreviatura,
        string Descripcion
    );
}