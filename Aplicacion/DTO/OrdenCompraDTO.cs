using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    public record OrdenCompraDTO(
        int id,
        string Descripcion,
        DateTime Fecha_Creacion,
        DateTime Fecha_Limite,
        DateTime? fecha_limite_ofertas,
        int Tipo_Orden,
        string Estado,
        int? id_UsuarioCreador 
    );
}