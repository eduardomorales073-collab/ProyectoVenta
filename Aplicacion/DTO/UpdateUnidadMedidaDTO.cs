using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateUnidadMedidaDTO(
        int id,
        string Nombre,
        string Abreviatura,
        string Descripcion
    );
}