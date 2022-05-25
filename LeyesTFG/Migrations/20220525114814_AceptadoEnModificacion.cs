using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeyesTFG.Migrations
{
    public partial class AceptadoEnModificacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Aceptado",
                table: "Modificacion",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aceptado",
                table: "Modificacion");
        }
    }
}
