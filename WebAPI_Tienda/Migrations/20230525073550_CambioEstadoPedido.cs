using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI_Tienda.Migrations
{
    public partial class CambioEstadoPedido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Confirmado",
                table: "Pedidos");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "Total",
                table: "Pedidos",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Pedidos");

            migrationBuilder.AddColumn<bool>(
                name: "Confirmado",
                table: "Pedidos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
