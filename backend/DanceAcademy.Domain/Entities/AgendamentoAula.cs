namespace DanceAcademy.Domain.Entities;

public class AgendamentoAula
{
    public Guid Id { get; set; }
    public Guid AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;
    
    public Guid TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
    
    public DateTime DataAula { get; set; } // Data específica da aula
    public bool PresencaConfirmada { get; set; } = true;
    
    public Guid? AulaOcorrenciaId { get; set; }
    public AulaOcorrencia? AulaOcorrencia { get; set; }
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
