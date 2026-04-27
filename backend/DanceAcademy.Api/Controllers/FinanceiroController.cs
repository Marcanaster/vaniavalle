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
public class FinanceiroController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FinanceiroController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("planos")]
    public async Task<IActionResult> GetPlanos()
    {
        var planos = await _context.Planos.ToListAsync();
        return Ok(planos);
    }

    [HttpPost("planos")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePlano([FromBody] PlanoCreateDto dto)
    {
        var plano = new Plano
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            Valor = dto.Valor,
            DuracaoMeses = dto.DuracaoMeses
        };
        _context.Planos.Add(plano);
        await _context.SaveChangesAsync();
        return Ok(plano);
    }

    [HttpGet("faturas")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetFaturas()
    {
        var faturas = await _context.Faturas.Include(f => f.Aluno).ToListAsync();
        return Ok(faturas);
    }

    [HttpGet("faturas/aluno/{alunoId}")]
    public async Task<IActionResult> GetFaturasAluno(Guid alunoId)
    {
        var faturas = await _context.Faturas
            .Where(f => f.AlunoId == alunoId)
            .ToListAsync();
        return Ok(faturas);
    }

    [HttpPost("faturas")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFatura([FromBody] FaturaCreateDto dto)
    {
        var fatura = new Fatura
        {
            Id = Guid.NewGuid(),
            AlunoId = dto.AlunoId,
            ValorTotal = dto.ValorTotal,
            DataVencimento = dto.DataVencimento,
            Status = "Pendente"
        };

        _context.Faturas.Add(fatura);
        await _context.SaveChangesAsync();

        return Ok(fatura);
    }

    [HttpPatch("faturas/{id}/pagar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PagarFatura(Guid id, [FromBody] FaturaPagarDto dto)
    {
        var fatura = await _context.Faturas.FindAsync(id);
        if (fatura == null) return NotFound();

        fatura.Status = "Pago";
        fatura.DataPagamento = DateTime.UtcNow;
        fatura.MetodoPagamento = dto.MetodoPagamento;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
