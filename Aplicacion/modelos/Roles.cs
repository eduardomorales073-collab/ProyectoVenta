using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Aplicacion.modelos
{
    public class Roles
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Externo { get; set; }
    }
}
