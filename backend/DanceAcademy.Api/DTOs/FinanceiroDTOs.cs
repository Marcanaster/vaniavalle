namespace DanceAcademy.Api.DTOs;

public class FaturaCreateDto
{
    public Guid AlunoId { get; set; }
    public DateTime DataVencimento { get; set; }
    public List<FaturaItemDto> Items { get; set; } = new();
}

public class FaturaItemDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal ValorBase { get; set; }
    public decimal DescontoPercentual { get; set; }
}

public class FaturaPagarDto
{
    public string MetodoPagamento { get; set; } = string.Empty;
}

public class PlanoCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int DuracaoMeses { get; set; }
}
