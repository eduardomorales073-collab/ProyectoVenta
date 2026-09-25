using System.ComponentModel.DataAnnotations;

namespace Aplicacion.modelos
{
    public class Departamento
    {
        [Key]
        public int id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int id_Sucursal { get; set; }
    }
}