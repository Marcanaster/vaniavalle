namespace DanceAcademy.Domain.Entities;

public class Plano
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: Mensal, Trimestral
    public decimal Valor { get; set; }
    public int DuracaoMeses { get; set; }

    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
}
