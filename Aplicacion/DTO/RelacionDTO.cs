namespace Aplicacion.DTO
{
    public record RelacionDTO(
        int id,
        int Proveedor1,
        string NombreProveedor1,
        int Proveedor2,
        string NombreProveedor2,
        string TipoRelacion
    );
}