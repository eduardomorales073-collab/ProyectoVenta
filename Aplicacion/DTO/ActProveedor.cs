using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record UpdateProveedorDTO(
        int id,
        string nombre,
        string Descripcion,
        string telefono,
        string Direccion,
        string? NIT,
        string? Categoria,
        List<int> id_Rubros
    );
}