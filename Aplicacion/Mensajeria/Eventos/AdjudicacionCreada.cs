namespace Aplicacion.Mensajeria.Eventos
{
    public record AdjudicacionCreada(
        int AdjudicacionId,
        int OrdenCompra,
        DateTime Fecha_Resolucion,
        string Estado,
        DateTime Fecha,
        string Usuario
    );
}