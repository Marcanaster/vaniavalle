namespace DanceAcademy.Api.DTOs;

public class AlunoCreateDto
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string ContatoEmergencia { get; set; } = string.Empty;
    
    // Endereço
    public string Cep { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;

    public string RestricoesSaude { get; set; } = string.Empty;
    public Guid PlanoId { get; set; }
    
    // Responsavel opcional
    public ResponsavelCreateDto? Responsavel { get; set; }
}

public class ResponsavelCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty; // CPF
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}

public class AlunoUpdateDto : AlunoCreateDto
{
}
