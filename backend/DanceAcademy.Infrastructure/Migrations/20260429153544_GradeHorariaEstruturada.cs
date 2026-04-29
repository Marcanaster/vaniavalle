using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanceAcademy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GradeHorariaEstruturada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sala",
                table: "Turmas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "AulaOcorrenciaId",
                table: "Agendamentos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AulasOcorrencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TurmaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    MotivoCancelamento = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AulasOcorrencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AulasOcorrencias_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurmasHorarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiaSemana = table.Column<int>(type: "integer", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TurmaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurmasHorarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurmasHorarios_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_AulaOcorrenciaId",
                table: "Agendamentos",
                column: "AulaOcorrenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_AulasOcorrencias_TurmaId",
                table: "AulasOcorrencias",
                column: "TurmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TurmasHorarios_TurmaId",
                table: "TurmasHorarios",
                column: "TurmaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_AulasOcorrencias_AulaOcorrenciaId",
                table: "Agendamentos",
                column: "AulaOcorrenciaId",
                principalTable: "AulasOcorrencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_AulasOcorrencias_AulaOcorrenciaId",
                table: "Agendamentos");

            migrationBuilder.DropTable(
                name: "AulasOcorrencias");

            migrationBuilder.DropTable(
                name: "TurmasHorarios");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_AulaOcorrenciaId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "Sala",
                table: "Turmas");

            migrationBuilder.DropColumn(
                name: "AulaOcorrenciaId",
                table: "Agendamentos");
        }
    }
}
