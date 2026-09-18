using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Datos.Configuraciones
{
    public class RolPermisoConfiguracion : IEntityTypeConfiguration<Rol_Permiso>
    {
        public void Configure(EntityTypeBuilder<Rol_Permiso> builder)
        {
            builder.ToTable("Rol_Permiso");
            builder.HasKey(r => new { r.id_Rol, r.id_Permiso });

            builder.HasOne<Roles>().WithMany().HasForeignKey(r => r.id_Rol);
            builder.HasOne<Permisos>().WithMany().HasForeignKey(r => r.id_Permiso);
        }
    }
}
