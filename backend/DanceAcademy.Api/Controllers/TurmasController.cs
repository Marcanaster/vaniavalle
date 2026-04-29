using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TurmasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTurmas()
    {
        var turmas = await _context.Turmas
            .Include(t => t.Modalidade)
            .Include(t => t.Professor)
            .Include(t => t.AlunosMatriculados)
                .ThenInclude(ta => ta.Aluno)
            .ToListAsync();
            
        return Ok(turmas);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTurma([FromBody] TurmaCreateDto dto)
    {
        var turma = new Turma
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Nivel = dto.Nivel,
            IdadeMinima = dto.IdadeMinima,
            IdadeMaxima = dto.IdadeMaxima,
            CapacidadeAlunos = dto.CapacidadeAlunos,
            GradeHorarios = dto.GradeHorarios,
            ModalidadeId = dto.ModalidadeId,
            ProfessorId = dto.ProfessorId
        };

        _context.Turmas.Add(turma);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTurmas), new { id = turma.Id }, turma);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTurma(Guid id, [FromBody] TurmaUpdateDto dto)
    {
        var turma = await _context.Turmas.FindAsync(id);
        if (turma == null) return NotFound();

        turma.Nome = dto.Nome;
        turma.Nivel = dto.Nivel;
        turma.IdadeMinima = dto.IdadeMinima;
        turma.IdadeMaxima = dto.IdadeMaxima;
        turma.CapacidadeAlunos = dto.CapacidadeAlunos;
        turma.GradeHorarios = dto.GradeHorarios;
        turma.ModalidadeId = dto.ModalidadeId;
        turma.ProfessorId = dto.ProfessorId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{turmaId}/matricular")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MatricularAluno(Guid turmaId, [FromBody] MatriculaRequestDto request)
    {
        var turma = await _context.Turmas.FindAsync(turmaId);
        var aluno = await _context.Alunos.FindAsync(request.AlunoId);

        if (turma == null || aluno == null) return NotFound("Turma ou Aluno não encontrados.");

        // Verificar se já está matriculado
        var jaMatriculado = await _context.TurmasAlunos
            .AnyAsync(ta => ta.TurmaId == turmaId && ta.AlunoId == request.AlunoId && ta.Ativo);

        if (jaMatriculado) return BadRequest(new { message = "Aluno já está matriculado nesta turma." });

        var matricula = new TurmaAluno
        {
            Id = Guid.NewGuid(),
            TurmaId = turmaId,
            AlunoId = request.AlunoId,
            DataMatricula = DateTime.UtcNow,
            Ativo = true,
            ValorMensal = request.ValorMensal,
            DescontoPercentual = request.DescontoPercentual,
            ValorMatricula = request.ValorMatricula
        };

        _context.TurmasAlunos.Add(matricula);

        // --- GERAÇÃO AUTOMÁTICA DE FATURA DE ADESÃO ---
        if (request.ValorMatricula > 0 || request.ValorMensal > 0)
        {
            var fatura = new Fatura
            {
                Id = Guid.NewGuid(),
                AlunoId = request.AlunoId,
                DataVencimento = DateTime.UtcNow.AddDays(5),
                Status = "Pendente",
                Items = new List<FaturaItem>()
            };

            if (request.ValorMatricula > 0)
            {
                fatura.Items.Add(new FaturaItem
                {
                    Id = Guid.NewGuid(),
                    Descricao = $"Matrícula - {turma.Nome}",
                    ValorBase = request.ValorMatricula,
                    DescontoPercentual = 0,
                    ValorFinal = request.ValorMatricula
                });
            }

            if (request.ValorMensal > 0)
            {
                var valorFinalMensal = request.ValorMensal * (1 - (request.DescontoPercentual / 100));
                fatura.Items.Add(new FaturaItem
                {
                    Id = Guid.NewGuid(),
                    Descricao = $"Mensalidade - {turma.Nome}",
                    ValorBase = request.ValorMensal,
                    DescontoPercentual = request.DescontoPercentual,
                    ValorFinal = valorFinalMensal
                });
            }

            fatura.ValorTotal = fatura.Items.Sum(i => i.ValorFinal);
            _context.Faturas.Add(fatura);
        }

        await _context.SaveChangesAsync();

        return Ok(matricula);
    }

    [HttpDelete("{turmaId}/desmatricular/{alunoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DesmatricularAluno(Guid turmaId, Guid alunoId)
    {
        var matricula = await _context.TurmasAlunos
            .FirstOrDefaultAsync(ta => ta.TurmaId == turmaId && ta.AlunoId == alunoId && ta.Ativo);

        if (matricula == null) return NotFound("Matrícula não encontrada.");

        matricula.Ativo = false; // Soft delete
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
