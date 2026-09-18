using Microsoft.EntityFrameworkCore;
using Aplicacion.modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Datos
{
    public class AplicacionDBContexto : DbContext
    {
        public DbSet<Adjudicacion> Adjudicacion { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<Detalle_Adjudicacion> Detalle_Adjudicacion { get; set; }
        public DbSet<Detalle_Pedido> Detalle_Pedido { get; set; }
        public DbSet<Detalle_Telefono> Detalle_Telefono { get; set; }
        public DbSet<Direccion> Direccion { get; set; }
        public DbSet<Oferta_Proveedor> Oferta_Proveedor { get; set; }
        public DbSet<Orden_Compra> Orden_Compra { get; set; }
        public DbSet<Pedido_Interno> Pedido_Interno { get; set; }
        public DbSet<Permisos> Permisos { get; set; }
        public DbSet<Provee_Artic> Provee_Artics { get; set; }
        public DbSet<Provee_Rubro> Provee_Rubros { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<Relacion> Relacion { get; set; }
        public DbSet<Rol_Permiso> Rol_Permiso { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Rubro> Rubro { get; set; }
        public DbSet<Sucursal> Sucursal { get; set; }
        public DbSet<Telefono> Telefono { get; set; }
        public DbSet<Tipo_Orden> Tipo_Orden { get; set; }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Usuarios_Roles> Usuarios_Roles { get; set; }

        public AplicacionDBContexto(DbContextOptions<AplicacionDBContexto> option) : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AplicacionDBContexto).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}