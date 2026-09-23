using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Pedido_Interno
    {
        public int id { get; set; }
        public int id_Departamento { get; set; }
        public int? id_OrdenCompra { get; set; }
        public DateTime Fecha_Solicitada { get; set; }
        public DateTime Fecha_Ingreso { get; set; }
    }
}