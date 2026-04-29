using DanceAcademy.Api.DTOs;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        // Alunos Ativos (O QueryFilter global já filtra apenas os ativos)
        var alunosAtivos = await _context.Alunos.CountAsync();
        
        // Turmas
        var turmasAtivas = await _context.Turmas.CountAsync();

        // Leads Pendentes de Ação (Aguardando contato ou aula)
        var leadsPendentes = await _context.AulasExperimentais
            .CountAsync(l => l.Status == "Pendente" || l.Status == "Agendada");

        // Receita Mês Atual
        var mesAtual = DateTime.UtcNow.Month;
        var anoAtual = DateTime.UtcNow.Year;

        var receitaMes = await _context.Faturas
            .Where(f => f.Status == "Pago" && f.DataPagamento.HasValue && f.DataPagamento.Value.Month == mesAtual && f.DataPagamento.Value.Year == anoAtual)
            .SumAsync(f => f.ValorTotal);

        // Aulas de Hoje
        var diaSemanaAtual = DateTime.UtcNow.DayOfWeek switch
        {
            DayOfWeek.Sunday => "Dom",
            DayOfWeek.Monday => "Seg",
            DayOfWeek.Tuesday => "Ter",
            DayOfWeek.Wednesday => "Qua",
            DayOfWeek.Thursday => "Qui",
            DayOfWeek.Friday => "Sex",
            DayOfWeek.Saturday => "Sab",
            _ => ""
        };

        var turmas = await _context.Turmas
            .Include(t => t.Agendamentos)
            .Where(t => t.GradeHorarios.Contains(diaSemanaAtual))
            .ToListAsync();

        var aulasHoje = turmas.Select(t => new TurmaHojeDto
        {
            Id = t.Id,
            Nome = t.Nome,
            Horario = ExtractTimeFromGrade(t.GradeHorarios),
            AlunosConfirmados = t.Agendamentos.Count(a => a.DataAula.Date == DateTime.UtcNow.Date && a.PresencaConfirmada)
        }).OrderBy(a => a.Horario).ToList();

        // Últimos Pagamentos
        var ultimosPagamentos = await _context.Faturas
            .Include(f => f.Aluno)
            .ThenInclude(a => a.Plano)
            .Where(f => f.Status == "Pago" && f.DataPagamento.HasValue)
            .OrderByDescending(f => f.DataPagamento)
            .Take(5)
            .Select(f => new FaturaRecenteDto
            {
                Id = f.Id,
                AlunoNome = f.Aluno.NomeCompleto,
                PlanoNome = f.Aluno.Plano != null ? f.Aluno.Plano.Nome : "Avulso",
                Iniciais = ObterIniciais(f.Aluno.NomeCompleto),
                Valor = f.ValorTotal,
                DataPagamento = f.DataPagamento.Value
            })
            .ToListAsync();

        var metrics = new DashboardMetricsDto
        {
            AlunosAtivos = alunosAtivos,
            TurmasAtivas = turmasAtivas,
            LeadsPendentes = leadsPendentes,
            ReceitaMes = receitaMes,
            AulasHoje = aulasHoje,
            UltimosPagamentos = ultimosPagamentos
        };

        return Ok(metrics);
    }

    private string ExtractTimeFromGrade(string grade)
    {
        if (string.IsNullOrWhiteSpace(grade)) return "??:??";
        var parts = grade.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts.Last() : "??:??";
    }

    private static string ObterIniciais(string nomeCompleto)
    {
        if (string.IsNullOrWhiteSpace(nomeCompleto)) return "??";
        var partes = nomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length == 1) return partes[0].Substring(0, Math.Min(2, partes[0].Length)).ToUpper();
        return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
    }

    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        return Ok(new
        {
            Alunos = await _context.Alunos.CountAsync(),
            Turmas = await _context.Turmas.CountAsync(),
            Modalidades = await _context.Modalidades.CountAsync(),
            Professores = await _context.Professores.CountAsync(),
            Faturas = await _context.Faturas.CountAsync(),
            ItensFatura = await _context.FaturaItems.CountAsync(),
            Matriculas = await _context.TurmasAlunos.CountAsync()
        });
    }
}
