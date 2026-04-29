namespace DanceAcademy.Domain.Entities;

public class AulaExperimental
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string TelefoneWhatsApp { get; set; } = string.Empty;
    public int? Idade { get; set; }
    public string? ModalidadeInteresse { get; set; }
    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataAgendada { get; set; }
    
    // Status: Pendente, Agendada, Realizada, Convertida, Cancelada
    public string Status { get; set; } = "Pendente"; 
    
    // Anotações extras enviadas pelo bot do n8n
    public string? ObservacoesAgent { get; set; } 
}
