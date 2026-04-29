using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinanceiroItemizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DescontoPercentual",
                table: "TurmasAlunos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorMensal",
                table: "TurmasAlunos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "FaturaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FaturaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    ValorBase = table.Column<decimal>(type: "numeric", nullable: false),
                    DescontoPercentual = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorFinal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaturaItems_Faturas_FaturaId",
                        column: x => x.FaturaId,
                        principalTable: "Faturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaturaItems_FaturaId",
                table: "FaturaItems",
                column: "FaturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaturaItems");

            migrationBuilder.DropColumn(
                name: "DescontoPercentual",
                table: "TurmasAlunos");

            migrationBuilder.DropColumn(
                name: "ValorMensal",
                table: "TurmasAlunos");
        }
    }
}
