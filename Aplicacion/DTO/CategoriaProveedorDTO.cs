using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record CategoriaProveedorDTO(
        int id,
        string nombre,
        string descripcion
    );
}