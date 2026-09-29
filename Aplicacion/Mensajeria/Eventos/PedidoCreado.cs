namespace Aplicacion.Mensajeria.Eventos
{
    public record PedidoCreado(
        int PedidoId,
        string Codigo,
        int IdDepartamento,
        int Cantidad,
        DateTime Fecha,
        string Usuario
    );
}