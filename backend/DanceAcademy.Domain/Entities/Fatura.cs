namespace DanceAcademy.Domain.Entities;

public class Fatura
{
    public Guid Id { get; set; }
    public Guid AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    
    public decimal ValorTotal { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    
    // Status: Pendente, Pago, Atrasado
    public string Status { get; set; } = "Pendente"; 
    
    // Metodo: Pix, Cartão, Dinheiro
    public string? MetodoPagamento { get; set; } 
}
