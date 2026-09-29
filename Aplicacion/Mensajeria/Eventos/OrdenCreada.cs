namespace Aplicacion.Mensajeria.Eventos
{
    public record OrdenCreada(
        int OrdenId,
        string Descripcion,
        DateTime Fecha_Creacion,
        DateTime Fecha_Limite,
        int Tipo_Orden
    );
}