using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixResponsavelConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_Documento",
                table: "Responsaveis",
                column: "Documento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responsaveis_Email",
                table: "Responsaveis",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Responsaveis_Documento",
                table: "Responsaveis");

            migrationBuilder.DropIndex(
                name: "IX_Responsaveis_Email",
                table: "Responsaveis");
        }
    }
}
