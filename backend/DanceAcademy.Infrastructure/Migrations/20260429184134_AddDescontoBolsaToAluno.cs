using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescontoBolsaToAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DescontoBolsa",
                table: "Alunos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescontoBolsa",
                table: "Alunos");
        }
    }
}
