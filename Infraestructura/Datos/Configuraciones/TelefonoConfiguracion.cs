using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Datos.Configuraciones
{
    public class TelefonoConfiguracion : IEntityTypeConfiguration<Telefono>
    {
        public void Configure(EntityTypeBuilder<Telefono> builder)
        {
            builder.ToTable("Telefono");
            builder.Property(t => t.Tel).HasColumnName("Telefono");
        }
    }
}