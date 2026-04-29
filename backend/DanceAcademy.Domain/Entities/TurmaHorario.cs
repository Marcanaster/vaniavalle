namespace DanceAcademy.Domain.Entities;

public class TurmaHorario
{
    public Guid Id { get; set; }
    
    // 0 = Domingo, 1 = Segunda, ..., 6 = Sábado
    public int DiaSemana { get; set; }
    
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    
    // Relacionamentos
    public Guid TurmaId { get; set; }
    public Turma Turma { get; set; } = null!;
}
