namespace DanceAcademy.Api.DTOs;

public class TurmaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public int IdadeMinima { get; set; }
    public int IdadeMaxima { get; set; }
    public int CapacidadeAlunos { get; set; }
    public string GradeHorarios { get; set; } = string.Empty; 
    public string Sala { get; set; } = string.Empty;
    public List<TurmaHorarioDto> Horarios { get; set; } = new();
    public Guid ModalidadeId { get; set; }
    public Guid? ProfessorId { get; set; }
}

public class TurmaHorarioDto
{
    public int DiaSemana { get; set; }
    public string HoraInicio { get; set; } = string.Empty; // HH:mm
    public string HoraFim { get; set; } = string.Empty; // HH:mm
}

public class TurmaUpdateDto : TurmaCreateDto { }

public class MatriculaRequestDto
{
    public Guid AlunoId { get; set; }
    public decimal ValorMensal { get; set; }
    public decimal DescontoPercentual { get; set; }
    public decimal ValorMatricula { get; set; }
}
