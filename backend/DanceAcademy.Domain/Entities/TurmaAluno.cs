namespace DanceAcademy.Domain.Entities;

public class TurmaAluno
{
    public Guid Id { get; set; }
    
    public Guid TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
    
    public Guid AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    
    public DateTime DataMatricula { get; set; } = DateTime.UtcNow;
    public bool Ativo { get; set; } = true;

    // Financeiro negociado
    public decimal ValorMensal { get; set; }
    public decimal DescontoPercentual { get; set; }
    public decimal ValorMatricula { get; set; }
}
