namespace DanceAcademy.Api.DTOs;

public class DashboardMetricsDto
{
    public int AlunosAtivos { get; set; }
    public int TurmasAtivas { get; set; }
    public int LeadsPendentes { get; set; }
    public decimal ReceitaMes { get; set; }
    public List<TurmaHojeDto> AulasHoje { get; set; } = new();
    public List<FaturaRecenteDto> UltimosPagamentos { get; set; } = new();
}

public class TurmaHojeDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Modalidade { get; set; } = string.Empty;
    public string Horario { get; set; } = string.Empty;
    public string Sala { get; set; } = string.Empty;
    public string Status { get; set; } = "Ativa"; // Ativa ou Cancelada
    public int AlunosConfirmados { get; set; }
}

public class FaturaRecenteDto
{
    public Guid Id { get; set; }
    public string AlunoNome { get; set; } = string.Empty;
    public string PlanoNome { get; set; } = string.Empty;
    public string Iniciais { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataPagamento { get; set; }
}
