namespace DanceAcademy.Domain.Entities;

public class Professor
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    
    public string? UserId { get; set; } // Vinculado ao IdentityUser para o login do professor
    
    public bool Ativo { get; set; } = true;
    public ICollection<Turma> Turmas { get; set; } = new List<Turma>();
}
