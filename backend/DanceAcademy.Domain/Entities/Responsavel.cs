namespace DanceAcademy.Domain.Entities;

public class Responsavel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty; // CPF
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    
    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
}
