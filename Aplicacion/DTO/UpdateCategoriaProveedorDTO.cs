using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateCategoriaProveedorDTO(
        int id,
        string Nombre,
        string Descripcion
    );
}