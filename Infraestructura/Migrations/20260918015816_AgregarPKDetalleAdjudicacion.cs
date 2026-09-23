using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPKDetalleAdjudicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "Adjudicacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha_Resolucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Orden_Compra = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjudicacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Departamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_Sucursal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Direccion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_Sucursal = table.Column<int>(type: "int", nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direccion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Oferta_Proveedor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_Proveedor = table.Column<int>(type: "int", nullable: false),
                    id_Pedido_Interno = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha_Oferta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oferta_Proveedor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Orden_Compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fecha_Limite = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo_Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orden_Compra", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Pedido_Interno",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_Departamento = table.Column<int>(type: "int", nullable: false),
                    Fecha_Solicitada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fecha_Ingreso = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido_Interno", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Crear = table.Column<bool>(type: "bit", nullable: false),
                    Leer = table.Column<bool>(type: "bit", nullable: false),
                    Actualizar = table.Column<bool>(type: "bit", nullable: false),
                    Borrar = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Provee_Rubros",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provee_Rubros", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Relacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Proveedor1 = table.Column<int>(type: "int", nullable: false),
                    Proveedor2 = table.Column<int>(type: "int", nullable: false),
                    TipoRelacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Externo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Rubro",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubro", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Sucursal",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursal", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Telefono",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefono", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Tipo_Orden",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grande = table.Column<bool>(type: "bit", nullable: false),
                    Urgente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Orden", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    id_Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Pedido",
                columns: table => new
                {
                    id_Pedido = table.Column<int>(type: "int", nullable: false),
                    id_Articulo = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Pedido", x => new { x.id_Pedido, x.id_Articulo });
                    table.ForeignKey(
                        name: "FK_Detalle_Pedido_Articulo_id_Articulo",
                        column: x => x.id_Articulo,
                        principalTable: "Articulo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalle_Pedido_Pedido_Interno_id_Pedido",
                        column: x => x.id_Pedido,
                        principalTable: "Pedido_Interno",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Adjudicacion",
                columns: table => new
                {
                    id_adjudicacion = table.Column<int>(type: "int", nullable: false),
                    id_Pedido = table.Column<int>(type: "int", nullable: false),
                    id_Proveedor = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Adjudicacion", x => new { x.id_adjudicacion, x.id_Pedido, x.id_Proveedor });
                    table.ForeignKey(
                        name: "FK_Detalle_Adjudicacion_Adjudicacion_id_adjudicacion",
                        column: x => x.id_adjudicacion,
                        principalTable: "Adjudicacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalle_Adjudicacion_Pedido_Interno_id_Pedido",
                        column: x => x.id_Pedido,
                        principalTable: "Pedido_Interno",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalle_Adjudicacion_Proveedor_id_Proveedor",
                        column: x => x.id_Proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Provee_Artic",
                columns: table => new
                {
                    id_Proveedor = table.Column<int>(type: "int", nullable: false),
                    id_Articulo = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provee_Artic", x => new { x.id_Proveedor, x.id_Articulo });
                    table.ForeignKey(
                        name: "FK_Provee_Artic_Articulo_id_Articulo",
                        column: x => x.id_Articulo,
                        principalTable: "Articulo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Provee_Artic_Proveedor_id_Proveedor",
                        column: x => x.id_Proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rol_Permiso",
                columns: table => new
                {
                    id_Rol = table.Column<int>(type: "int", nullable: false),
                    id_Permiso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol_Permiso", x => new { x.id_Rol, x.id_Permiso });
                    table.ForeignKey(
                        name: "FK_Rol_Permiso_Permisos_id_Permiso",
                        column: x => x.id_Permiso,
                        principalTable: "Permisos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rol_Permiso_Roles_id_Rol",
                        column: x => x.id_Rol,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Telefono",
                columns: table => new
                {
                    id_Sucursal = table.Column<int>(type: "int", nullable: false),
                    id_Telefono = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Telefono", x => new { x.id_Sucursal, x.id_Telefono });
                    table.ForeignKey(
                        name: "FK_Detalle_Telefono_Sucursal_id_Sucursal",
                        column: x => x.id_Sucursal,
                        principalTable: "Sucursal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Detalle_Telefono_Telefono_id_Telefono",
                        column: x => x.id_Telefono,
                        principalTable: "Telefono",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios_Roles",
                columns: table => new
                {
                    id_Usuario = table.Column<int>(type: "int", nullable: false),
                    id_Rol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Roles", x => new { x.id_Usuario, x.id_Rol });
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_Roles_id_Rol",
                        column: x => x.id_Rol,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_Usuarios_id_Usuario",
                        column: x => x.id_Usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Adjudicacion_id_Pedido",
                table: "Detalle_Adjudicacion",
                column: "id_Pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Adjudicacion_id_Proveedor",
                table: "Detalle_Adjudicacion",
                column: "id_Proveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Pedido_id_Articulo",
                table: "Detalle_Pedido",
                column: "id_Articulo");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Telefono_id_Telefono",
                table: "Detalle_Telefono",
                column: "id_Telefono");

            migrationBuilder.CreateIndex(
                name: "IX_Provee_Artic_id_Articulo",
                table: "Provee_Artic",
                column: "id_Articulo");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Permiso_id_Permiso",
                table: "Rol_Permiso",
                column: "id_Permiso");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Roles_id_Rol",
                table: "Usuarios_Roles",
                column: "id_Rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departamento");

            migrationBuilder.DropTable(
                name: "Detalle_Adjudicacion");

            migrationBuilder.DropTable(
                name: "Detalle_Pedido");

            migrationBuilder.DropTable(
                name: "Detalle_Telefono");

            migrationBuilder.DropTable(
                name: "Direccion");

            migrationBuilder.DropTable(
                name: "Oferta_Proveedor");

            migrationBuilder.DropTable(
                name: "Orden_Compra");

            migrationBuilder.DropTable(
                name: "Provee_Artic");

            migrationBuilder.DropTable(
                name: "Provee_Rubros");

            migrationBuilder.DropTable(
                name: "Relacion");

            migrationBuilder.DropTable(
                name: "Rol_Permiso");

            migrationBuilder.DropTable(
                name: "Rubro");

            migrationBuilder.DropTable(
                name: "Tipo_Orden");

            migrationBuilder.DropTable(
                name: "Usuarios_Roles");

            migrationBuilder.DropTable(
                name: "Adjudicacion");

            migrationBuilder.DropTable(
                name: "Pedido_Interno");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropTable(
                name: "Telefono");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            
        }
    }
}
