namespace Aplicacion.Mensajeria.Eventos
{
    public record CompraRegistrada(
        Guid OrdenId,
        string Detalle,
        decimal Monto,
        DateTime Fecha,
        string Proveedor
    );
}