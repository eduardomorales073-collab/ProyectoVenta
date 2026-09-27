using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UnidadMedidaDTO(
        int id,
        string nombre,
        string abreviatura,
        string descripcion
    );
}