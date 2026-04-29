using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/frequencia")]
[Authorize] // Admin ou Teacher
public class FrequenciaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FrequenciaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("minhas-turmas")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMinhasTurmas()
    {
        // Obter o ID do usuário autenticado no JWT
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var professor = await _context.Professores.FirstOrDefaultAsync(p => p.UserId == userId);
        if (professor == null) return Forbid();

        var turmas = await _context.Turmas
            .Where(t => t.ProfessorId == professor.Id)
            .Select(t => new {
                t.Id,
                t.Nome,
                t.GradeHorarios,
                TotalAlunos = t.AlunosMatriculados.Count(ta => ta.Ativo)
            })
            .ToListAsync();

        return Ok(turmas);
    }

    [HttpGet("{turmaId}")]
    public async Task<IActionResult> GetFrequencia(Guid turmaId, [FromQuery] DateTime data)
    {
        var turma = await _context.Turmas.FindAsync(turmaId);
        if (turma == null) return NotFound();

        // Pegar todos os alunos matriculados ativos nesta turma
        var matriculas = await _context.TurmasAlunos
            .Include(ta => ta.Aluno)
            .Where(ta => ta.TurmaId == turmaId && ta.Ativo && ta.Aluno.Ativo)
            .ToListAsync();

        var alunoIds = matriculas.Select(m => m.AlunoId).ToList();

        // Pegar os registros de agendamento (presença) para essa turma e data
        var presencas = await _context.Agendamentos
            .Where(ag => ag.TurmaId == turmaId && ag.DataAula.Date == data.Date && alunoIds.Contains(ag.AlunoId))
            .ToListAsync();

        // Montar a resposta mesclando matriculados com o status de presença atual
        var resultado = matriculas.Select(m => new
        {
            AlunoId = m.AlunoId,
            NomeAluno = m.Aluno.NomeCompleto,
            Presente = presencas.FirstOrDefault(p => p.AlunoId == m.AlunoId)?.PresencaConfirmada
        }).OrderBy(x => x.NomeAluno).ToList();

        return Ok(resultado);
    }

    public class SalvarChamadaDto
    {
        public DateTime Data { get; set; }
        public List<AlunoPresencaDto> Alunos { get; set; } = new();
    }

    public class AlunoPresencaDto
    {
        public Guid AlunoId { get; set; }
        public bool Presente { get; set; }
    }

    [HttpPost("{turmaId}")]
    public async Task<IActionResult> SalvarFrequencia(Guid turmaId, [FromBody] SalvarChamadaDto dto)
    {
        var turma = await _context.Turmas.FindAsync(turmaId);
        if (turma == null) return NotFound();

        var alunoIds = dto.Alunos.Select(a => a.AlunoId).ToList();

        // Buscar registros existentes
        var registrosExistentes = await _context.Agendamentos
            .Where(ag => ag.TurmaId == turmaId && ag.DataAula.Date == dto.Data.Date && alunoIds.Contains(ag.AlunoId))
            .ToListAsync();

        foreach (var alunoDto in dto.Alunos)
        {
            var registro = registrosExistentes.FirstOrDefault(r => r.AlunoId == alunoDto.AlunoId);
            if (registro != null)
            {
                registro.PresencaConfirmada = alunoDto.Presente;
            }
            else
            {
                _context.Agendamentos.Add(new AgendamentoAula
                {
                    Id = Guid.NewGuid(),
                    TurmaId = turmaId,
                    AlunoId = alunoDto.AlunoId,
                    DataAula = dto.Data.Date,
                    PresencaConfirmada = alunoDto.Presente,
                    DataCriacao = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Chamada salva com sucesso." });
    }
}
