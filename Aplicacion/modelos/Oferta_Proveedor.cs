using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Oferta_Proveedor
    {
        public int id { get; set; }
        public int id_Proveedor { get; set; }
        public int id_Pedido_Interno { get; set; }
        public decimal Precio { get; set; }
        public DateTime Fecha_Oferta { get; set; }

    }
}
