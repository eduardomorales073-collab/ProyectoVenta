using Aplicacion.modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Datos.Configuraciones
{
    public class UsuariosRolesConfiguracion : IEntityTypeConfiguration<Usuarios_Roles>
    {
        public void Configure(EntityTypeBuilder<Usuarios_Roles> builder)
        {
            builder.ToTable("Usuarios_Roles");
            builder.HasKey(u => new { u.id_Usuario, u.id_Rol });

            builder.HasOne<Usuarios>().WithMany().HasForeignKey(u => u.id_Usuario);
            builder.HasOne<Roles>().WithMany().HasForeignKey(u => u.id_Rol);
        }
    }
}
