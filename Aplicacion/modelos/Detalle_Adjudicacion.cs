using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Detalle_Adjudicacion
    {
        public int id_adjudicacion { get; set; }
        public int id_Pedido { get; set; }
        public int id_Proveedor { get; set; }
        public decimal Precio { get; set; }
        public int  Cantidad { get; set; }
    }
}
