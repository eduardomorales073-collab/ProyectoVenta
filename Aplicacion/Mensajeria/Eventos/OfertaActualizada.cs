namespace Aplicacion.Mensajeria.Eventos
{
    public record OfertaActualizada(
        int OfertaId,
        int IdProveedor,
        int IdPedidoInterno,
        decimal PrecioAnterior,
        decimal PrecioNuevo,
        DateTime Fecha,
        string Proveedor
    );
}