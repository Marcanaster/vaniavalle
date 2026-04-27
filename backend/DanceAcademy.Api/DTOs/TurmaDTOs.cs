namespace DanceAcademy.Api.DTOs;

public class TurmaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty;
    public int IdadeMinima { get; set; }
    public int IdadeMaxima { get; set; }
    public int CapacidadeAlunos { get; set; }
    public string GradeHorarios { get; set; } = string.Empty; 
    public Guid ModalidadeId { get; set; }
}

public class TurmaUpdateDto : TurmaCreateDto { }

public class ModalidadeCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
}
