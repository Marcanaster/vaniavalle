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
    public async Task<IActionResult> GetMetrics([FromQuery] int? mes = null, [FromQuery] int? ano = null)
    {
        var mesAlvo = mes ?? DateTime.UtcNow.Month;
        var anoAlvo = ano ?? DateTime.UtcNow.Year;

        // Alunos Ativos
        var alunosAtivos = await _context.Alunos.CountAsync();
        
        // Turmas
        var turmasAtivas = await _context.Turmas.CountAsync();

        // Leads Pendentes de Ação (Aguardando contato ou aula)
        var leadsPendentes = await _context.AulasExperimentais
            .CountAsync(l => l.Status == "Pendente" || l.Status == "Agendada");

        var receitaMes = await _context.Faturas
            .Where(f => f.Status == "Pago" && f.DataPagamento.HasValue && f.DataPagamento.Value.Month == mesAlvo && f.DataPagamento.Value.Year == anoAlvo)
            .SumAsync(f => f.ValorTotal);

        var receitaPrevistaMes = await _context.Faturas
            .Where(f => f.DataVencimento.Month == mesAlvo && f.DataVencimento.Year == anoAlvo)
            .SumAsync(f => f.ValorTotal);

        // Inadimplência (Faturas não pagas e vencidas até hoje)
        var hojeData = DateTime.UtcNow.Date;
        var faturasVencidas = await _context.Faturas
            .Where(f => f.Status != "Pago" && f.DataVencimento.Date < hojeData)
            .ToListAsync();

        var inadimplenciaTotal = faturasVencidas.Sum(f => f.ValorTotal);
        var alunosInadimplentes = faturasVencidas.Select(f => f.AlunoId).Distinct().Count();

        // Dados para o Gráfico de Receita e Inadimplência (6 meses terminando no mês selecionado)
        var receitaMensalChart = new List<ChartDataDto>();
        var inadimplenciaMensalChart = new List<ChartDataDto>();

        var dataInicioChart = new DateTime(anoAlvo, mesAlvo, 1).AddMonths(-5);
        var dataFimChart = new DateTime(anoAlvo, mesAlvo, 1).AddMonths(1).AddDays(-1);

        // Busca todas as faturas relevantes para o período de 6 meses em uma única query
        // Relevantes se: vencem no período OU foram pagas no período
        var faturasPeriodo = await _context.Faturas
            .Where(f => (f.DataVencimento >= dataInicioChart && f.DataVencimento <= dataFimChart) ||
                        (f.DataPagamento.HasValue && f.DataPagamento.Value >= dataInicioChart && f.DataPagamento.Value <= dataFimChart))
            .ToListAsync();

        for (int i = 5; i >= 0; i--)
        {
            var inicioMes = dataInicioChart.AddMonths(5 - i);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);
            var nomeMes = inicioMes.ToString("MMM", new CultureInfo("pt-BR")).ToUpper();

            // Arrecadado: O que foi pago DENTRO deste mês (Fluxo de Caixa)
            var valorArrecadado = faturasPeriodo
                .Where(f => f.Status == "Pago" && f.DataPagamento.HasValue && 
                            f.DataPagamento.Value.Date >= inicioMes && f.DataPagamento.Value.Date <= fimMes)
                .Sum(f => f.ValorTotal);

            // Previsto: O que vence NESTE mês (Faturamento)
            var valorPrevisto = faturasPeriodo
                .Where(f => f.DataVencimento.Date >= inicioMes && f.DataVencimento.Date <= fimMes)
                .Sum(f => f.ValorTotal);

            // Inadimplência (Snapshot Histórico)
            decimal valorInadimplencia = 0;
            
            // Se o mês em questão é o mês atual (ou futuro em relação ao servidor)
            if (inicioMes.Year == hojeData.Year && inicioMes.Month == hojeData.Month)
            {
                // Mês Corrente: Apenas o que já venceu e ainda não foi pago
                valorInadimplencia = faturasPeriodo
                    .Where(f => f.DataVencimento.Date >= inicioMes && f.DataVencimento.Date <= fimMes && 
                                f.Status != "Pago" && f.DataVencimento.Date < hojeData)
                    .Sum(f => f.ValorTotal);
            }
            else if (inicioMes < hojeData)
            {
                // Mês Passado: Congelamento (O que não estava pago no último dia do mês)
                valorInadimplencia = faturasPeriodo
                    .Where(f => f.DataVencimento.Date >= inicioMes && f.DataVencimento.Date <= fimMes && 
                                (f.Status != "Pago" || (f.DataPagamento.HasValue && f.DataPagamento.Value.Date > fimMes)))
                    .Sum(f => f.ValorTotal);
            }

            receitaMensalChart.Add(new ChartDataDto
            {
                Label = nomeMes,
                Value = valorArrecadado,
                SecondaryValue = valorPrevisto
            });

            inadimplenciaMensalChart.Add(new ChartDataDto
            {
                Label = nomeMes,
                Value = valorInadimplencia
            });
        }

        // Aulas de Hoje (Sempre usa UTC-3 para o Brasil se necessário, mas aqui usaremos o padrão do servidor)
        var hoje = DateTime.Today;
        var diaSemanaInt = (int)hoje.DayOfWeek;

        // 1. Buscar ocorrências já geradas para hoje (incluindo as canceladas)
        var ocorrenciasHoje = await _context.AulasOcorrencias
            .Include(o => o.Turma)
            .ThenInclude(t => t.Modalidades)
            .Include(o => o.Presencas)
            .Where(o => o.DataHora.Date == hoje)
            .ToListAsync();

        // 2. Buscar turmas que deveriam ter aula hoje mas ainda não tem ocorrência gerada
        var turmasComHorarioHoje = await _context.Turmas
            .Include(t => t.Modalidades)
            .Include(t => t.Horarios)
            .Include(t => t.AlunosMatriculados)
            .Where(t => t.Horarios.Any(h => h.DiaSemana == diaSemanaInt))
            .ToListAsync();

        var aulasHoje = new List<AulaHojeDto>();

        // Adicionar ocorrências existentes
        foreach (var oc in ocorrenciasHoje)
        {
            aulasHoje.Add(new AulaHojeDto
            {
                Id = oc.Id,
                Nome = oc.Turma.Nome,
                Modalidade = string.Join(", ", oc.Turma.Modalidades.Select(m => m.Nome)),
                Horario = oc.DataHora.ToString("HH:mm"),
                Sala = oc.Turma.Sala,
                Status = oc.Status,
                AlunosConfirmados = oc.Presencas.Count(p => p.PresencaConfirmada)
            });
        }

        // Adicionar turmas que ainda não tem ocorrência (considerar ativas)
        foreach (var turma in turmasComHorarioHoje)
        {
            var horariosHoje = turma.Horarios.Where(h => h.DiaSemana == diaSemanaInt);
            foreach (var h in horariosHoje)
            {
                // Se já existe ocorrência para este horário, pula
                if (ocorrenciasHoje.Any(oc => oc.TurmaId == turma.Id && oc.DataHora.TimeOfDay == h.HoraInicio))
                    continue;

                aulasHoje.Add(new AulaHojeDto
                {
                    Id = turma.Id,
                    Nome = turma.Nome,
                    Modalidade = string.Join(", ", turma.Modalidades.Select(m => m.Nome)),
                    Horario = h.HoraInicio.ToString(@"hh\:mm"),
                    Sala = turma.Sala,
                    Status = "Ativa",
                    AlunosConfirmados = turma.AlunosMatriculados.Count(m => m.Ativo)
                });
            }
        }

        aulasHoje = aulasHoje.OrderBy(a => a.Horario).ToList();

        // Últimos Pagamentos
        var ultimosPagamentos = await _context.Faturas
            .Include(f => f.Aluno)
            .ThenInclude(a => a.Plano)
            .Where(f => f.Status == "Pago" && f.DataPagamento.HasValue)
            .OrderByDescending(f => f.DataPagamento)
            .Take(5)
            .Select(f => new UltimoPagamentoDto
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
            ReceitaPrevistaMes = receitaPrevistaMes,
            InadimplenciaTotal = inadimplenciaTotal,
            AlunosInadimplentes = alunosInadimplentes,
            ReceitaMensalChart = receitaMensalChart,
            InadimplenciaMensalChart = inadimplenciaMensalChart,
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
