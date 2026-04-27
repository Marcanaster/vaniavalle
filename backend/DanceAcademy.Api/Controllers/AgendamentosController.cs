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
public class AgendamentosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AgendamentosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("turma/{turmaId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAgendamentosTurma(Guid turmaId)
    {
        var agendamentos = await _context.Agendamentos
            .Include(a => a.Aluno)
            .Where(a => a.TurmaId == turmaId)
            .ToListAsync();
        return Ok(agendamentos);
    }
    
    [HttpGet("aluno/{alunoId}")]
    public async Task<IActionResult> GetAgendamentosAluno(Guid alunoId)
    {
        var agendamentos = await _context.Agendamentos
            .Include(a => a.Turma)
            .Where(a => a.AlunoId == alunoId)
            .ToListAsync();
        return Ok(agendamentos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAgendamento([FromBody] AgendamentoCreateDto dto)
    {
        var agendamento = new AgendamentoAula
        {
            Id = Guid.NewGuid(),
            AlunoId = dto.AlunoId,
            TurmaId = dto.TurmaId,
            DataAula = dto.DataAula,
            PresencaConfirmada = true,
            DataCriacao = DateTime.UtcNow
        };

        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();

        return Ok(agendamento);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelarAgendamento(Guid id)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null) return NotFound();

        _context.Agendamentos.Remove(agendamento);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
