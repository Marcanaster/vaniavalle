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

            // Regra: Prevalece o maior desconto (Bolsa do Aluno vs Desconto da Turma)
            var descontoEfetivo = Math.Max(aluno.DescontoBolsa, request.DescontoPercentual);

            if (request.ValorMatricula > 0)
            {
                fatura.Items.Add(new FaturaItem
                {
                    Id = Guid.NewGuid(),
                    Descricao = $"Taxa de Matrícula - {turma.Nome}",
                    ValorBase = request.ValorMatricula,
                    DescontoPercentual = 0,
                    ValorFinal = request.ValorMatricula
                });
            }

            if (request.ValorMensal > 0)
            {
                fatura.Items.Add(new FaturaItem
                {
                    Id = Guid.NewGuid(),
                    Descricao = $"Primeira Mensalidade (Pro-rata/Adesão) - {turma.Nome}",
                    ValorBase = request.ValorMensal,
                    DescontoPercentual = descontoEfetivo,
                    ValorFinal = request.ValorMensal * (1 - (descontoEfetivo / 100))
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
    [HttpPost("{turmaId}/regerar-fatura/{alunoId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegerarFatura(Guid turmaId, Guid alunoId)
    {
        var turma = await _context.Turmas.FindAsync(turmaId);
        var aluno = await _context.Alunos.FindAsync(alunoId);
        var matricula = await _context.TurmasAlunos
            .FirstOrDefaultAsync(ta => ta.TurmaId == turmaId && ta.AlunoId == alunoId && ta.Ativo);

        if (turma == null || aluno == null || matricula == null) 
            return NotFound("Vínculo não encontrado.");

        // 1. Remover faturas pendentes que contenham esta turma
        var faturasPendentes = await _context.Faturas
            .Include(f => f.Items)
            .Where(f => f.AlunoId == alunoId && f.Status == "Pendente")
            .ToListAsync();

        foreach (var f in faturasPendentes)
        {
            if (f.Items.Any(i => i.Descricao.Contains(turma.Nome)))
            {
                _context.Faturas.Remove(f);
            }
        }

        // 2. Gerar nova fatura com regras atualizadas
        var fatura = new Fatura
        {
            Id = Guid.NewGuid(),
            AlunoId = alunoId,
            DataVencimento = DateTime.UtcNow.AddDays(5),
            Status = "Pendente",
            Items = new List<FaturaItem>()
        };

        var descontoEfetivo = Math.Max(aluno.DescontoBolsa, matricula.DescontoPercentual);

        fatura.Items.Add(new FaturaItem
        {
            Id = Guid.NewGuid(),
            Descricao = $"Fatura Recalculada - {turma.Nome}",
            ValorBase = matricula.ValorMensal,
            DescontoPercentual = descontoEfetivo,
            ValorFinal = matricula.ValorMensal * (1 - (descontoEfetivo / 100))
        });

        fatura.ValorTotal = fatura.Items.Sum(i => i.ValorFinal);
        _context.Faturas.Add(fatura);

        await _context.SaveChangesAsync();

        return Ok(new { message = "Fatura regerada com sucesso!" });
    }
}
