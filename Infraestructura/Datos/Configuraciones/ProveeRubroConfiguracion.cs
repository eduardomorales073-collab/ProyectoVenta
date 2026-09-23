using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class ProveeRubroConfiguracion : IEntityTypeConfiguration<Provee_Rubro>
    {
        public void Configure(EntityTypeBuilder<Provee_Rubro> builder)
        {
            builder.ToTable("Provee_Rubro");
            builder.HasKey(p => new { p.id_Proveedor, p.id_Rubro });

            builder.HasOne<Proveedor>().WithMany().HasForeignKey(p => p.id_Proveedor);
            builder.HasOne<Rubro>().WithMany().HasForeignKey(p => p.id_Rubro);
        }
    }
}