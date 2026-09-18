using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Adjudicacion
    {
        public int id { get; set; }
        public DateTime Fecha_Resolucion { get; set; }
        public int Orden_Compra { get; set; }
        public string Estado { get; set; }

    }
}
