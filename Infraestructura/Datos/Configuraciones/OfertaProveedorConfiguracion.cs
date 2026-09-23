using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class OfertaProveedorConfiguracion : IEntityTypeConfiguration<Oferta_Proveedor>
    {
        public void Configure(EntityTypeBuilder<Oferta_Proveedor> builder)
        {
            builder.ToTable("Oferta_Proveedor");
            builder.Property(o => o.Precio).HasColumnType("decimal(18,2)");
        }
    }
}