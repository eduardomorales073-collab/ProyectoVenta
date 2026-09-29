namespace Aplicacion.Mensajeria.Eventos
{
    public record OfertaRegistrada(
        int OfertaId,
        int IdProveedor,
        int IdPedidoInterno,
        decimal Precio,
        DateTime Fecha,
        string Proveedor
    );
}