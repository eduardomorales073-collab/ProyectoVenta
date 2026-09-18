using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class DetalleTelefonoConfiguracion : IEntityTypeConfiguration<Detalle_Telefono>
    {
        public void Configure(EntityTypeBuilder<Detalle_Telefono> builder)
        {
            builder.ToTable("Detalle_Telefono");

            builder.HasKey(d => new { d.id_Sucursal, d.id_Telefono });

            builder.HasOne<Sucursal>()
                   .WithMany()
                   .HasForeignKey(d => d.id_Sucursal);

            builder.HasOne<Telefono>()
                   .WithMany()
                   .HasForeignKey(d => d.id_Telefono);
        }
    }
}