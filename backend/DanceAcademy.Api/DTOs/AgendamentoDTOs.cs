namespace DanceAcademy.Api.DTOs;

public class AgendamentoCreateDto
{
    public Guid AlunoId { get; set; }
    public Guid TurmaId { get; set; }
    public DateTime DataAula { get; set; }
}
