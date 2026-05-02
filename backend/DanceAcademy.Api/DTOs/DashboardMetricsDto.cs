namespace DanceAcademy.Api.DTOs;

public class DashboardMetricsDto
{
    public int AlunosAtivos { get; set; }
    public int TurmasAtivas { get; set; }
    public int LeadsPendentes { get; set; }
    public decimal ReceitaMes { get; set; }
    public decimal ReceitaPrevistaMes { get; set; }
    public decimal InadimplenciaTotal { get; set; }
    public int AlunosInadimplentes { get; set; }
    public List<ChartDataDto> ReceitaMensalChart { get; set; } = new();
    public List<ChartDataDto> InadimplenciaMensalChart { get; set; } = new();
    public List<AulaHojeDto> AulasHoje { get; set; } = new();
    public List<UltimoPagamentoDto> UltimosPagamentos { get; set; } = new();
}

public class ChartDataDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? SecondaryValue { get; set; }
}

public class AulaHojeDto
{
    public Guid Id { get; set; }
    public string Horario { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Modalidade { get; set; } = string.Empty;
    public string? Sala { get; set; }
    public int AlunosConfirmados { get; set; }
    public string Status { get; set; } = "Normal"; // Normal, Cancelada
}

public class UltimoPagamentoDto
{
    public Guid Id { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string Iniciais { get; set; } = string.Empty;
    public string PlanoNome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
}
