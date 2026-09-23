using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class PedidoInternoConfiguracion : IEntityTypeConfiguration<Pedido_Interno>
    {
        public void Configure(EntityTypeBuilder<Pedido_Interno> builder)
        {
            builder.ToTable("Pedido_Interno");
            builder.HasKey(p => p.id);

            builder.HasOne<Orden_Compra>()
                   .WithMany()
                   .HasForeignKey(p => p.id_OrdenCompra)
                   .IsRequired(false);
        }
    }
}