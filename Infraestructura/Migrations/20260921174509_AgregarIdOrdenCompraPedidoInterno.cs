using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdOrdenCompraPedidoInterno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Provee_Rubros",
                table: "Provee_Rubros");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Provee_Rubros");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Provee_Rubros");

            migrationBuilder.RenameTable(
                name: "Provee_Rubros",
                newName: "Provee_Rubro");

            migrationBuilder.RenameColumn(
                name: "Tel",
                table: "Telefono",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Provee_Rubro",
                newName: "id_Rubro");

            migrationBuilder.AddColumn<int>(
                name: "id_OrdenCompra",
                table: "Pedido_Interno",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_Rubro",
                table: "Provee_Rubro",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "id_Proveedor",
                table: "Provee_Rubro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provee_Rubro",
                table: "Provee_Rubro",
                columns: new[] { "id_Proveedor", "id_Rubro" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Interno_id_OrdenCompra",
                table: "Pedido_Interno",
                column: "id_OrdenCompra");

            migrationBuilder.CreateIndex(
                name: "IX_Provee_Rubro_id_Rubro",
                table: "Provee_Rubro",
                column: "id_Rubro");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Interno_Orden_Compra_id_OrdenCompra",
                table: "Pedido_Interno",
                column: "id_OrdenCompra",
                principalTable: "Orden_Compra",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Provee_Rubro_Proveedor_id_Proveedor",
                table: "Provee_Rubro",
                column: "id_Proveedor",
                principalTable: "Proveedor",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Provee_Rubro_Rubro_id_Rubro",
                table: "Provee_Rubro",
                column: "id_Rubro",
                principalTable: "Rubro",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Interno_Orden_Compra_id_OrdenCompra",
                table: "Pedido_Interno");

            migrationBuilder.DropForeignKey(
                name: "FK_Provee_Rubro_Proveedor_id_Proveedor",
                table: "Provee_Rubro");

            migrationBuilder.DropForeignKey(
                name: "FK_Provee_Rubro_Rubro_id_Rubro",
                table: "Provee_Rubro");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_Interno_id_OrdenCompra",
                table: "Pedido_Interno");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Provee_Rubro",
                table: "Provee_Rubro");

            migrationBuilder.DropIndex(
                name: "IX_Provee_Rubro_id_Rubro",
                table: "Provee_Rubro");

            migrationBuilder.DropColumn(
                name: "id_OrdenCompra",
                table: "Pedido_Interno");

            migrationBuilder.DropColumn(
                name: "id_Proveedor",
                table: "Provee_Rubro");

            migrationBuilder.RenameTable(
                name: "Provee_Rubro",
                newName: "Provee_Rubros");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Telefono",
                newName: "Tel");

            migrationBuilder.RenameColumn(
                name: "id_Rubro",
                table: "Provee_Rubros",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "Provee_Rubros",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Provee_Rubros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Provee_Rubros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Provee_Rubros",
                table: "Provee_Rubros",
                column: "id");
        }
    }
}
