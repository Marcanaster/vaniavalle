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
    public string Sala { get; set; } = string.Empty; // Ex: Sala 1, Estúdio A

    // Relacionamentos
    public ICollection<TurmaHorario> Horarios { get; set; } = new List<TurmaHorario>();
    public ICollection<Modalidade> Modalidades { get; set; } = new List<Modalidade>();
    
    public Guid? ProfessorId { get; set; }
    public Professor? Professor { get; set; }
    
    public ICollection<AgendamentoAula> Agendamentos { get; set; } = new List<AgendamentoAula>();
    public ICollection<TurmaAluno> AlunosMatriculados { get; set; } = new List<TurmaAluno>();
}
