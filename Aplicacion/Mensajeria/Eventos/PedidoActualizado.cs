namespace Aplicacion.Mensajeria.Eventos
{
    public record PedidoActualizado(
        int PedidoId,
        string Codigo,
        int Cantidad,
        int IdDepartamento,
        DateTime Fecha,
        string Usuario
    );
}