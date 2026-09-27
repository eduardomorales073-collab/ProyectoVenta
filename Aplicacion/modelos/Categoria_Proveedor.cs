using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplicacion.modelos
{
    public class Categoria_Proveedor
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}