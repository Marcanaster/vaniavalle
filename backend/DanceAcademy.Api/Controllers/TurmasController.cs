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
        var turmas = await _context.Turmas.Include(t => t.Modalidade).ToListAsync();
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
            ModalidadeId = dto.ModalidadeId
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

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
