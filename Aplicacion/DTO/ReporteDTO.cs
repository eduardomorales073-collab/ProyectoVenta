using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.DTO
{
    
        public record HistorialCompraDTO(DateTime FechaResolucion, string Proveedor, decimal Precio);

        public record RankingProveedorDTO(int IdProveedor, string Nombre, decimal TotalAdjudicado);

        public record OfertaComparativaDTO(int IdProveedor, string Proveedor, decimal Precio, DateTime FechaOferta, bool EsGanador);

        public record PedidoPendienteDTO(int Id, int IdDepartamento, DateTime FechaSolicitada);

        public record OrdenActivaDTO(int Id, string Descripcion, DateTime Fecha_Creacion, DateTime Fecha_Limite);

        public record GastoDepartamentalDTO(int IdDepartamento, string NombreDepartamento, decimal TotalGastado);

        public record EficienciaCompraDTO(double PromedioDias);

        public record VariacionPrecioDTO(DateTime Fecha, decimal Precio);
    
}
