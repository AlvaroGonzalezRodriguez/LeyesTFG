using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeyesTFG.Migrations
{
    public partial class PendienteEva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PendienteEva",
                table: "Modificacion",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendienteEva",
                table: "Modificacion");
        }
    }
}
