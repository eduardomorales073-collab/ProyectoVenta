using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Datos.Configuraciones
{
    public class ProveeArticConfiguracion : IEntityTypeConfiguration<Provee_Artic>
    {
        public void Configure(EntityTypeBuilder<Provee_Artic> builder)
        {
            builder.ToTable("Provee_Artic");
            builder.HasKey(p => new { p.id_Proveedor, p.id_Articulo });
            builder.Property(p => p.Precio).HasColumnType("decimal(18,2)");

            builder.HasOne<Proveedor>().WithMany().HasForeignKey(p => p.id_Proveedor);
            builder.HasOne<Articulo>().WithMany().HasForeignKey(p => p.id_Articulo);
        }
    }
}
