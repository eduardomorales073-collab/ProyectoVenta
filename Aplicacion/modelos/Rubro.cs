using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Aplicacion.modelos
{
    public class Rubro
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}