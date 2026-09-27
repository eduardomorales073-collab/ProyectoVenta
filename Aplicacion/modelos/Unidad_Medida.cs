using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplicacion.modelos
{
    public class Unidad_Medida
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public string Descripcion { get; set; }
    }
}