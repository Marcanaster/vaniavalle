namespace DanceAcademy.Domain.Entities;

public class AulaOcorrencia
{
    public Guid Id { get; set; }
    
    public Guid TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
    
    public DateTime DataHora { get; set; }
    
    // Status: "Ativa", "Cancelada"
    public string Status { get; set; } = "Ativa";
    public string? MotivoCancelamento { get; set; }
    
    // Relacionamentos
    public ICollection<AgendamentoAula> Presencas { get; set; } = new List<AgendamentoAula>();
}
