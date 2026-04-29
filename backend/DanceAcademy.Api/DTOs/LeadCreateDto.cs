namespace DanceAcademy.Api.DTOs;

public class LeadCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string TelefoneWhatsApp { get; set; } = string.Empty;
    public int? Idade { get; set; }
    public string? ModalidadeInteresse { get; set; }
    public DateTime? DataAgendada { get; set; }
    public string? ObservacoesAgent { get; set; }
    public string? Status { get; set; }
}
