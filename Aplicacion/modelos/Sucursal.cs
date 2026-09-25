using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aplicacion.modelos
{
    public class Sucursal
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
    }
}