using System.ComponentModel.DataAnnotations.Schema;

namespace DanceAcademy.Domain.Entities;

public class FaturaItem
{
    public Guid Id { get; set; }
    public Guid FaturaId { get; set; }
    
    [ForeignKey("FaturaId")]
    public Fatura? Fatura { get; set; }

    public string Descricao { get; set; } = string.Empty;
    public decimal ValorBase { get; set; }
    public decimal DescontoPercentual { get; set; }
    public decimal ValorFinal { get; set; }
}
