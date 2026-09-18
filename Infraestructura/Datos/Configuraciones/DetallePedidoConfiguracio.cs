using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class DetallePedidoConfiguracion : IEntityTypeConfiguration<Detalle_Pedido>
    {
        public void Configure(EntityTypeBuilder<Detalle_Pedido> builder)
        {
            builder.ToTable("Detalle_Pedido");

            builder.HasKey(d => new { d.id_Pedido, d.id_Articulo });

            builder.HasOne<Pedido_Interno>()
                   .WithMany()
                   .HasForeignKey(d => d.id_Pedido);

            builder.HasOne<Articulo>()
                   .WithMany()
                   .HasForeignKey(d => d.id_Articulo);
        }
    }
}