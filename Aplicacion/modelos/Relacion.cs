using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Relacion
    {
        public int id { get; set; }
        public int Proveedor1 { get; set; }
        public int Proveedor2 { get; set; }
        public string TipoRelacion { get; set; }
    }
}
