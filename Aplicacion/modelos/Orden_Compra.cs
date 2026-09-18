using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Orden_Compra
    {
        public int id { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_Limite { get; set; }
        public int Tipo_Orden { get; set; }

    }
}
