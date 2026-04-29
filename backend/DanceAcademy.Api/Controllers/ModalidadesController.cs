using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/modalidades")]
[Authorize]
public class ModalidadesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ModalidadesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var modalidades = await _context.Modalidades.OrderBy(m => m.Nome).ToListAsync();
        return Ok(modalidades);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ModalidadeCreateDto dto)
    {
        var modalidade = new Modalidade
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Descricao = dto.Descricao
        };

        _context.Modalidades.Add(modalidade);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = modalidade.Id }, modalidade);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ModalidadeCreateDto dto)
    {
        var modalidade = await _context.Modalidades.FindAsync(id);
        if (modalidade == null) return NotFound();

        modalidade.Nome = dto.Nome;
        modalidade.Descricao = dto.Descricao;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var modalidade = await _context.Modalidades.FindAsync(id);
        if (modalidade == null) return NotFound();

        _context.Modalidades.Remove(modalidade);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
