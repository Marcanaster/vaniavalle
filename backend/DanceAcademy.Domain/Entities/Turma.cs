namespace DanceAcademy.Domain.Entities;

public class Turma
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty; // Ex: Ballet Infantil
    public string Nivel { get; set; } = string.Empty; // Ex: Iniciante, Intermediário
    public int IdadeMinima { get; set; }
    public int IdadeMaxima { get; set; }
    public int CapacidadeAlunos { get; set; }
    
    // Horários da turma (simplificado como string para o MVP, ex: "Ter/Qui 18:00")
    public string GradeHorarios { get; set; } = string.Empty; 

    // Relacionamentos
    public Guid ModalidadeId { get; set; }
    public Modalidade Modalidade { get; set; } = null!;
    
    public Guid? ProfessorId { get; set; }
    public Professor? Professor { get; set; }
    
    public ICollection<AgendamentoAula> Agendamentos { get; set; } = new List<AgendamentoAula>();
    public ICollection<TurmaAluno> AlunosMatriculados { get; set; } = new List<TurmaAluno>();
}
