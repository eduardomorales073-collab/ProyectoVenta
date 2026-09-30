using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Aplicacion.modelos
{
    public class Telefono
    {
        public int id { get; set; }

        // La columna en la BD se llama "Telefono"
        [Column("Telefono")]
        public string Tel { get; set; }

        public DateTime Fecha { get; set; }
    }
}