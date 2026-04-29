namespace DanceAcademy.Domain.Entities;

public class Aluno
{
    public Guid Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty; // Pode ser vazio caso seja criança
    public DateTime DataNascimento { get; set; }
    public string ContatoEmergencia { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    
    // Endereço
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    // Exclusão Lógica (Soft Delete)
    public bool Ativo { get; set; } = true;
    public DateTime? DataExclusao { get; set; }

    // Questionário de Saúde/Anamnese
    public string RestricoesSaude { get; set; } = string.Empty;
    public string? UserId { get; set; }
    
    // Relacionamentos
    public Guid? ResponsavelId { get; set; } // Nullable caso o aluno seja adulto e o próprio responsável
    public Responsavel? Responsavel { get; set; }

    public Guid PlanoId { get; set; }
    public Plano Plano { get; set; } = null!;
    public int DiaVencimento { get; set; } = 5;
    public decimal DescontoBolsa { get; set; } = 0; // Porcentagem de bolsa fixa
    public ICollection<Fatura> Faturas { get; set; } = new List<Fatura>();
    public ICollection<AgendamentoAula> Agendamentos { get; set; } = new List<AgendamentoAula>();
    public ICollection<TurmaAluno> Turmas { get; set; } = new List<TurmaAluno>();
}
