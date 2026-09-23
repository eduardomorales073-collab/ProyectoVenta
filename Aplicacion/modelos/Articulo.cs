using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplicacion.modelos
{
    public class Articulo
    {
        [Key]
        public int id { get; set; }      // ← sin DatabaseGenerated

        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}