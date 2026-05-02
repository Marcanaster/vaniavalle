using DanceAcademy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DanceAcademy.Domain.Entities;

namespace DanceAcademy.Api.Services;

public class FinanceiroBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FinanceiroBackgroundService> _logger;

    public FinanceiroBackgroundService(IServiceProvider serviceProvider, ILogger<FinanceiroBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço Financeiro Background iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await GerarFaturasAutomaticas(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no processamento automático de faturas.");
            }

            // Executa uma vez por dia (24 horas)
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task GerarFaturasAutomaticas(ApplicationDbContext context)
    {
        var hoje = DateTime.UtcNow;
        var alunos = await context.Alunos
            .Include(a => a.Turmas)
                .ThenInclude(ta => ta.Turma)
            .Where(a => a.Ativo)
            .ToListAsync();

        foreach (var aluno in alunos)
        {
            var turmasAtivas = aluno.Turmas.Where(t => t.Ativo).ToList();
            if (!turmasAtivas.Any()) continue;

            // Mês Alvo: Se o vencimento do aluno no mês atual ou no próximo estiver a menos de 10 dias, gera.
            var mesesParaChecar = new List<DateTime> { 
                new DateTime(hoje.Year, hoje.Month, 1),
                new DateTime(hoje.AddMonths(1).Year, hoje.AddMonths(1).Month, 1) 
            };

            foreach (var refMes in mesesParaChecar)
            {
                DateTime vencimento;
                try {
                    vencimento = new DateTime(refMes.Year, refMes.Month, aluno.DiaVencimento);
                } catch {
                    vencimento = new DateTime(refMes.Year, refMes.Month, DateTime.DaysInMonth(refMes.Year, refMes.Month));
                }

                // REGRA: Só gera a fatura se faltar EXATAMENTE 10 dias para o vencimento (ou menos, caso o robô tenha ficado offline)
                var diasParaVencimento = (vencimento.Date - hoje.Date).TotalDays;
                if (diasParaVencimento > 10 || diasParaVencimento < -5) continue; 

                // Não gera se já existir fatura de MENSALIDADE para este mês/ano
                var jaExiste = await context.Faturas
                    .Include(f => f.Items)
                    .AnyAsync(f => f.AlunoId == aluno.Id && 
                                   f.DataVencimento.Month == refMes.Month && 
                                   f.DataVencimento.Year == refMes.Year &&
                                   f.Items.Any(i => i.Descricao.Contains("Mensalidade")));

                if (jaExiste) continue;

                var fatura = new Fatura
                {
                    Id = Guid.NewGuid(),
                    AlunoId = aluno.Id,
                    DataVencimento = vencimento,
                    Status = "Pendente",
                    Items = turmasAtivas.Select(ta => {
                        var descontoFinal = Math.Max(aluno.DescontoBolsa, ta.DescontoPercentual);
                        return new FaturaItem
                        {
                            Id = Guid.NewGuid(),
                            Descricao = $"Mensalidade - {ta.Turma.Nome}",
                            ValorBase = ta.ValorMensal,
                            DescontoPercentual = descontoFinal,
                            ValorFinal = ta.ValorMensal * (1 - (descontoFinal / 100))
                        };
                    }).ToList()
                };

                fatura.ValorTotal = fatura.Items.Sum(i => i.ValorFinal);

                if (fatura.ValorTotal == 0)
                {
                    fatura.Status = "Pago";
                    fatura.DataPagamento = DateTime.UtcNow;
                    fatura.MetodoPagamento = "Isento";
                }

                context.Faturas.Add(fatura);
            }
        }

        await context.SaveChangesAsync();
    }
}
