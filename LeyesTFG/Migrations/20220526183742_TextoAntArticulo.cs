using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeyesTFG.Migrations
{
    public partial class TextoAntArticulo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextoAnterior",
                table: "Articulo",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextoAnterior",
                table: "Articulo");
        }
    }
}
