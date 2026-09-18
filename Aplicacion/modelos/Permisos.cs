using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.modelos
{
    public class Permisos
    {
        public int id { get; set; }
        public bool Crear { get; set; }
        public bool Leer { get; set; }
        public bool Actualizar { get; set; }
        public bool Borrar { get; set; }
        public DateTime Fecha { get; set; }
    }
}
