using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplicacion.modelos
{
    public class Articulo
    {
        [Key]
        public int id { get; set; }

        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        // ===== NUEVOS CAMPOS =====
        public string? codigo { get; set; }
        public string? unidad_medida { get; set; }
    }
}