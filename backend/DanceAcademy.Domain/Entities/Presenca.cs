namespace DanceAcademy.Domain.Entities;

public class Presenca
{
    public Guid Id { get; set; }
    public Guid TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
    
    public Guid AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    
    public DateTime Data { get; set; }
    public bool Presente { get; set; } = true;
    public string? Observacao { get; set; }
}
