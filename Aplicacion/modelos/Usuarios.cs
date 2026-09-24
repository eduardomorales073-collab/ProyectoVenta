using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aplicacion.modelos
{
    public class Usuarios
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public string email { get; set; }
        public string Contrasena { get; set; }
        public bool Activo { get; set; }
        public int id_Rol { get; set; }
    }
}