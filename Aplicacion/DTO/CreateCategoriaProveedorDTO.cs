using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CreateCategoriaProveedorDTO(
        string Nombre,
        string Descripcion
    );
}