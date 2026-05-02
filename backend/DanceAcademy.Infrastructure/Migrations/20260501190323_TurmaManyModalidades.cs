using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TurmaManyModalidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModalidadeTurma",
                columns: table => new
                {
                    ModalidadesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurmasId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalidadeTurma", x => new { x.ModalidadesId, x.TurmasId });
                    table.ForeignKey(
                        name: "FK_ModalidadeTurma_Modalidades_ModalidadesId",
                        column: x => x.ModalidadesId,
                        principalTable: "Modalidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModalidadeTurma_Turmas_TurmasId",
                        column: x => x.TurmasId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrar dados existentes antes de deletar a coluna
            migrationBuilder.Sql("INSERT INTO \"ModalidadeTurma\" (\"ModalidadesId\", \"TurmasId\") SELECT \"ModalidadeId\", \"Id\" FROM \"Turmas\" WHERE \"ModalidadeId\" IS NOT NULL AND \"ModalidadeId\" <> '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.DropForeignKey(
                name: "FK_Turmas_Modalidades_ModalidadeId",
                table: "Turmas");

            migrationBuilder.DropIndex(
                name: "IX_Turmas_ModalidadeId",
                table: "Turmas");

            migrationBuilder.DropColumn(
                name: "ModalidadeId",
                table: "Turmas");

            migrationBuilder.CreateIndex(
                name: "IX_ModalidadeTurma_TurmasId",
                table: "ModalidadeTurma",
                column: "TurmasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModalidadeId",
                table: "Turmas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Tentar restaurar a primeira modalidade encontrada antes de apagar a tabela de junção
            migrationBuilder.Sql("UPDATE \"Turmas\" SET \"ModalidadeId\" = (SELECT \"ModalidadesId\" FROM \"ModalidadeTurma\" WHERE \"TurmasId\" = \"Turmas\".\"Id\" LIMIT 1)");

            migrationBuilder.DropTable(
                name: "ModalidadeTurma");

            migrationBuilder.CreateIndex(
                name: "IX_Turmas_ModalidadeId",
                table: "Turmas",
                column: "ModalidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turmas_Modalidades_ModalidadeId",
                table: "Turmas",
                column: "ModalidadeId",
                principalTable: "Modalidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
