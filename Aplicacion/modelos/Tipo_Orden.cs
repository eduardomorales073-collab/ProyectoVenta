        using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Aplicacion.modelos
{
    public class Tipo_Orden
    {
        [Key]
        public int id { get; set; }
        public bool Grande { get; set; }
        public bool Urgente { get; set; }
    }
}
