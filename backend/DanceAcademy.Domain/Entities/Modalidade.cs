namespace DanceAcademy.Domain.Entities;

public class Modalidade
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: Ballet, Jazz
    public string Descricao { get; set; } = string.Empty;
    
    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
}
