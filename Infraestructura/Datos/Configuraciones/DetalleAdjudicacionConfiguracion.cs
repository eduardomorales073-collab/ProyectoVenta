using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class DetalleAdjudicacionConfiguracion : IEntityTypeConfiguration<Detalle_Adjudicacion>
    {
        public void Configure(EntityTypeBuilder<Detalle_Adjudicacion> builder)
        {
            builder.ToTable("Detalle_Adjudicacion");

            builder.HasKey(d => new { d.id_adjudicacion, d.id_Pedido, d.id_Proveedor });

            builder.Property(d => d.Precio)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne<Adjudicacion>()
                   .WithMany()
                   .HasForeignKey(d => d.id_adjudicacion);

            builder.HasOne<Pedido_Interno>()
                   .WithMany()
                   .HasForeignKey(d => d.id_Pedido);

            builder.HasOne<Proveedor>()
                   .WithMany()
                   .HasForeignKey(d => d.id_Proveedor);
        }
    }
}