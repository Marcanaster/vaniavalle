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
        var faturas = await _context.Faturas
            .Include(f => f.Aluno)
            .Include(f => f.Items)
            .OrderByDescending(f => f.DataVencimento)
            .ToListAsync();
        return Ok(faturas);
    }

    [HttpGet("faturas/aluno/{alunoId}")]
    public async Task<IActionResult> GetFaturasAluno(Guid alunoId)
    {
        var faturas = await _context.Faturas
            .Include(f => f.Items)
            .Where(f => f.AlunoId == alunoId)
            .OrderByDescending(f => f.DataVencimento)
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
            DataVencimento = dto.DataVencimento,
            Status = "Pendente",
            Items = dto.Items.Select(i => new FaturaItem
            {
                Id = Guid.NewGuid(),
                Descricao = i.Descricao,
                ValorBase = i.ValorBase,
                DescontoPercentual = i.DescontoPercentual,
                ValorFinal = i.ValorBase * (1 - (i.DescontoPercentual / 100))
            }).ToList()
        };

        fatura.ValorTotal = fatura.Items.Sum(i => i.ValorFinal);

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

    [HttpPost("faturas/gerar-mensais")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GerarMensalidades()
    {
        var alunos = await _context.Alunos
            .Include(a => a.Turmas)
                .ThenInclude(ta => ta.Turma)
            .Where(a => a.Ativo)
            .ToListAsync();

        var count = 0;
        var hoje = DateTime.UtcNow;

        foreach (var aluno in alunos)
        {
            var turmasAtivas = aluno.Turmas.Where(t => t.Ativo && t.ValorMensal > 0).ToList();
            if (!turmasAtivas.Any()) continue;

            // Define o mês de referência (se hoje > dia 20, gera para o mês que vem?)
            // Para o MVP: Gera para o mês atual se não houver. 
            // Se hoje estiver nos últimos 10 dias do mês, verifica também o mês que vem.
            
            var mesesParaChecar = new List<DateTime> { new DateTime(hoje.Year, hoje.Month, 1) };
            if (hoje.Day > 20) mesesParaChecar.Add(new DateTime(hoje.AddMonths(1).Year, hoje.AddMonths(1).Month, 1));

            foreach (var refMes in mesesParaChecar)
            {
                var jaExiste = await _context.Faturas
                    .AnyAsync(f => f.AlunoId == aluno.Id && 
                                   f.DataVencimento.Month == refMes.Month && 
                                   f.DataVencimento.Year == refMes.Year &&
                                   f.Items.Any(i => i.Descricao.Contains("Mensalidade")));

                if (jaExiste) continue;

                DateTime vencimento;
                try {
                    vencimento = new DateTime(refMes.Year, refMes.Month, aluno.DiaVencimento);
                } catch {
                    vencimento = new DateTime(refMes.Year, refMes.Month, DateTime.DaysInMonth(refMes.Year, refMes.Month));
                }

                // Se o vencimento for no passado (ex: hoje é dia 15 e vencimento era dia 5), mantém para registro
                
                var fatura = new Fatura
                {
                    Id = Guid.NewGuid(),
                    AlunoId = aluno.Id,
                    DataVencimento = vencimento,
                    Status = "Pendente",
                    Items = turmasAtivas.Select(ta => new FaturaItem
                    {
                        Id = Guid.NewGuid(),
                        Descricao = $"Mensalidade - {ta.Turma.Nome}",
                        ValorBase = ta.ValorMensal,
                        DescontoPercentual = ta.DescontoPercentual,
                        ValorFinal = ta.ValorMensal * (1 - (ta.DescontoPercentual / 100))
                    }).ToList()
                };

                fatura.ValorTotal = fatura.Items.Sum(i => i.ValorFinal);
                _context.Faturas.Add(fatura);
                count++;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = $"{count} faturas de mensalidade geradas.", Count = count });
    }
}
