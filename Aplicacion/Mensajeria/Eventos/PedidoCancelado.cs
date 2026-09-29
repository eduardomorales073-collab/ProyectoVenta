namespace Aplicacion.Mensajeria.Eventos
{
    public record PedidoCancelado(
        int PedidoId,
        string Codigo,
        string Motivo,
        DateTime Fecha,
        string Usuario
    );
}