using System;
using System.Collections.Generic;

namespace Aplicacion.DTO
{
    public class ProveedorRelacionadoOfertaDTO
    {
        public string NombreProveedor { get; set; }
        public string TipoRelacion { get; set; }
        public decimal PrecioOfertado { get; set; }
    }

    public class OfertaProveedorDTO
    {
        public int id { get; set; }
        public int id_Pedido_Interno { get; set; }
        public int id_Proveedor { get; set; }
        public decimal precio { get; set; }
        public DateTime fecha_Oferta { get; set; }

        public bool PedidoAdjudicado { get; set; }
        public List<ProveedorRelacionadoOfertaDTO> ProveedoresRelacionadosQueOfertaron { get; set; } = new List<ProveedorRelacionadoOfertaDTO>();
    }
}