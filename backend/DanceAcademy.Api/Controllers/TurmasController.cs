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
            .Include(t => t.Horarios)
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
            Sala = dto.Sala,
            ModalidadeId = dto.ModalidadeId,
            ProfessorId = dto.ProfessorId,
            Horarios = dto.Horarios.Select(h => new TurmaHorario
            {
                Id = Guid.NewGuid(),
                DiaSemana = h.DiaSemana,
                HoraInicio = TimeSpan.Parse(h.HoraInicio),
                HoraFim = TimeSpan.Parse(h.HoraFim)
            }).ToList()
        };

        _context.Turmas.Add(turma);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTurmas), new { id = turma.Id }, turma);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTurma(Guid id, [FromBody] TurmaUpdateDto dto)
    {
        var turma = await _context.Turmas.Include(t => t.Horarios).FirstOrDefaultAsync(t => t.Id == id);
        if (turma == null) return NotFound();

        // 1. Atualizar campos básicos
        turma.Nome = dto.Nome;
        turma.Nivel = dto.Nivel;
        turma.IdadeMinima = dto.IdadeMinima;
        turma.IdadeMaxima = dto.IdadeMaxima;
        turma.CapacidadeAlunos = dto.CapacidadeAlunos;
        turma.GradeHorarios = dto.GradeHorarios;
        turma.Sala = dto.Sala;
        turma.ModalidadeId = dto.ModalidadeId;
        turma.ProfessorId = dto.ProfessorId;

        // 2. Limpar horários antigos e salvar para garantir a remoção
        if (turma.Horarios.Any())
        {
            _context.TurmasHorarios.RemoveRange(turma.Horarios);
            await _context.SaveChangesAsync();
        }

        // 3. Adicionar novos horários
        if (dto.Horarios != null && dto.Horarios.Any())
        {
            var novosHorarios = dto.Horarios.Select(h => new TurmaHorario
            {
                Id = Guid.NewGuid(),
                TurmaId = id,
                DiaSemana = h.DiaSemana,
                HoraInicio = TimeSpan.Parse(h.HoraInicio),
                HoraFim = TimeSpan.Parse(h.HoraFim)
            }).ToList();

            _context.TurmasHorarios.AddRange(novosHorarios);
            await _context.SaveChangesAsync();
        }
        else 
        {
            // Se não houver novos horários, apenas salvamos as mudanças nos campos básicos (caso não tenha entrado no if acima)
            await _context.SaveChangesAsync();
        }

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
