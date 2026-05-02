using System.Security.Claims;
using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Domain.Interfaces;
using DanceAcademy.Domain.Helpers;
using DanceAcademy.Infrastructure.Data;
using DanceAcademy.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requer estar logado
public class AlunosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AlunosController> _logger;


    public AlunosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailService emailService, ILogger<AlunosController> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAlunos()
    {
        var alunos = await _context.Alunos
            .Include(a => a.Plano)
            .Include(a => a.Responsavel)
            .ToListAsync();
            
        return Ok(alunos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAluno(Guid id)
    {
        // Para MVP: Todos logam, mas em prod o aluno id deve bater com a claim
        var aluno = await _context.Alunos
            .Include(a => a.Plano)
            .Include(a => a.Responsavel)
            .Include(a => a.Faturas)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aluno == null) return NotFound();

        return Ok(aluno);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAluno([FromBody] AlunoCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var aluno = new Aluno
            {
                Id = Guid.NewGuid(),
                NomeCompleto = dto.NomeCompleto,
                Cpf = dto.Cpf,
                DataNascimento = dto.DataNascimento,
                ContatoEmergencia = dto.ContatoEmergencia,
                Telefone = dto.Telefone,
                Cep = dto.Cep,
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                RestricoesSaude = dto.RestricoesSaude,
                PlanoId = dto.PlanoId,
                DiaVencimento = dto.DiaVencimento,
                DescontoBolsa = dto.DescontoBolsa,
                Ativo = true
            };

            // Lógica de Responsável e Acesso (Obrigatório)
            if (dto.Responsavel == null || string.IsNullOrWhiteSpace(dto.Responsavel.Email) || string.IsNullOrWhiteSpace(dto.Responsavel.Documento))
            {
                return BadRequest(new { error = "Dados do responsável financeiro (E-mail e CPF) são obrigatórios." });
            }

            // Tenta encontrar responsável existente pelo CPF ou e-mail
            var responsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => 
                r.Documento == dto.Responsavel.Documento || r.Email == dto.Responsavel.Email);

            if (responsavel == null)
            {
                responsavel = new Responsavel
                {
                    Id = Guid.NewGuid(),
                    Nome = dto.Responsavel.Nome,
                    Documento = dto.Responsavel.Documento,
                    Email = dto.Responsavel.Email,
                    Telefone = dto.Responsavel.Telefone
                };
                _context.Responsaveis.Add(responsavel);
            }
            else
            {
                // Atualiza dados de contato caso tenham mudado (opcional, mas bom para fluidez)
                responsavel.Nome = dto.Responsavel.Nome;
                responsavel.Telefone = dto.Responsavel.Telefone;
                // E-mail e CPF não mudamos aqui pois são chaves, se mudar o usuário teria que ser outro ou via admin
            }
            
            aluno.ResponsavelId = responsavel.Id;

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Gerenciar Usuário de Acesso (Sempre vinculado ao Responsável)
            string emailLogin = responsavel.Email;
            string? userId = null;
            string? generatedPassword = null;
            
            var existingUser = await _userManager.FindByEmailAsync(emailLogin);
            if (existingUser == null)
            {
                generatedPassword = PasswordHelper.GenerateRandomPassword();
                var user = new IdentityUser { UserName = emailLogin, Email = emailLogin };
                var result = await _userManager.CreateAsync(user, generatedPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    userId = user.Id;
                    responsavel.UserId = userId;
                }
                else
                {
                    throw new Exception("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                userId = existingUser.Id;
                if (responsavel.UserId == null)
                {
                    responsavel.UserId = userId;
                }
                // Garante que o usuário existente tenha a role Student para acessar o portal
                if (!await _userManager.IsInRoleAsync(existingUser, "Student"))
                {
                    await _userManager.AddToRoleAsync(existingUser, "Student");
                }
            }

            aluno.UserId = userId;
            await _context.SaveChangesAsync();

            // E-mail de Boas-vindas (Apenas para NOVOS usuários)
            if (userId != null && generatedPassword != null)
            {
                string htmlContent = EmailTemplates.GetWelcomeTemplate(responsavel.Nome, emailLogin, generatedPassword, "Student");
                try 
                { 
                    await _emailService.SendEmailAsync(emailLogin, "Bem-vindo à Dance Academy Vania Valle", htmlContent); 
                }
                catch (Exception ex) 
                { 
                    _logger.LogError(ex, "Erro ao enviar e-mail de boas-vindas para {Email}", emailLogin); 
                }
            }

            await transaction.CommitAsync();
            return CreatedAtAction(nameof(GetAluno), new { id = aluno.Id }, aluno);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAluno(Guid id, [FromBody] AlunoUpdateDto dto)
    {
        var aluno = await _context.Alunos.Include(a => a.Responsavel).FirstOrDefaultAsync(a => a.Id == id);
        if (aluno == null) return NotFound();

        aluno.NomeCompleto = dto.NomeCompleto;
        aluno.Cpf = dto.Cpf;
        aluno.DataNascimento = dto.DataNascimento;
        aluno.ContatoEmergencia = dto.ContatoEmergencia;
        aluno.Telefone = dto.Telefone;
        aluno.Cep = dto.Cep;
        aluno.Logradouro = dto.Logradouro;
        aluno.Numero = dto.Numero;
        aluno.Complemento = dto.Complemento;
        aluno.Bairro = dto.Bairro;
        aluno.Cidade = dto.Cidade;
        aluno.Estado = dto.Estado;
        aluno.RestricoesSaude = dto.RestricoesSaude;
        aluno.PlanoId = dto.PlanoId;

        if (dto.Responsavel != null)
        {
            // Tenta encontrar responsável existente pelo CPF ou e-mail caso tenha mudado
            var existingResponsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => 
                r.Documento == dto.Responsavel.Documento || r.Email == dto.Responsavel.Email);

            if (aluno.Responsavel != null)
            {
                // Se o responsável já é o mesmo (pelo ID ou pelas chaves), apenas atualiza
                if (existingResponsavel != null && existingResponsavel.Id != aluno.ResponsavelId)
                {
                    // Se encontrou outro responsável com esses dados, troca a vinculação do aluno
                    aluno.ResponsavelId = existingResponsavel.Id;
                }
                else
                {
                    // Caso contrário, atualiza os dados do responsável atual
                    aluno.Responsavel.Nome = dto.Responsavel.Nome;
                    aluno.Responsavel.Documento = dto.Responsavel.Documento;
                    aluno.Responsavel.Email = dto.Responsavel.Email;
                    aluno.Responsavel.Telefone = dto.Responsavel.Telefone;
                }
            }
            else
            {
                if (existingResponsavel != null)
                {
                    aluno.ResponsavelId = existingResponsavel.Id;
                }
                else
                {
                    var r = new Responsavel
                    {
                        Id = Guid.NewGuid(),
                        Nome = dto.Responsavel.Nome,
                        Documento = dto.Responsavel.Documento,
                        Email = dto.Responsavel.Email,
                        Telefone = dto.Responsavel.Telefone
                    };
                    _context.Responsaveis.Add(r);
                    aluno.ResponsavelId = r.Id;
                }
            }
        }

        // Sincronizar UserId se o responsável mudou ou se estava faltando
        var responsavelFinal = aluno.Responsavel ?? await _context.Responsaveis.FindAsync(aluno.ResponsavelId);
        if (responsavelFinal != null)
        {
            if (responsavelFinal.UserId == null)
            {
                var user = await _userManager.FindByEmailAsync(responsavelFinal.Email);
                if (user != null)
                {
                    responsavelFinal.UserId = user.Id;
                    if (!await _userManager.IsInRoleAsync(user, "Student"))
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                    }
                }
            }
            aluno.UserId = responsavelFinal.UserId;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAluno(Guid id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno == null) return NotFound();

        // Soft Delete
        aluno.Ativo = false;
        aluno.DataExclusao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("meu-perfil")]
    [Authorize]
    public async Task<IActionResult> GetMeuPerfil()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        List<Aluno> alunos = new List<Aluno>();
        string nomeExibicao = "Aluno";

        // 1. Tentar encontrar como Responsável (Família)
        var responsavel = await _context.Responsaveis
            .Include(r => r.Alunos)
                .ThenInclude(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Modalidades)
            .Include(r => r.Alunos)
                .ThenInclude(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Horarios)
            .Include(r => r.Alunos)
                .ThenInclude(a => a.Faturas)
            .FirstOrDefaultAsync(r => r.UserId == userId);

        if (responsavel != null)
        {
            alunos = responsavel.Alunos.ToList();
            nomeExibicao = responsavel.Nome;
        }
        else
        {
            // 2. Tentar encontrar como Aluno Individual (Adulto)
            var alunoIndividual = await _context.Alunos
                .Include(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Modalidades)
                .Include(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Horarios)
                .Include(a => a.Faturas)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (alunoIndividual != null)
            {
                alunos.Add(alunoIndividual);
                nomeExibicao = alunoIndividual.NomeCompleto;
            }
            else if (User.IsInRole("Admin"))
            {
                // Fallback para Admin testando
                var primeiroAluno = await _context.Alunos
                    .Include(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Modalidades)
                    .Include(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Horarios)
                    .Include(a => a.Faturas)
                    .FirstOrDefaultAsync();
                if (primeiroAluno != null) alunos.Add(primeiroAluno);
                nomeExibicao = "Administrador (Simulando Aluno)";
            }
        }

        return Ok(new {
            NomeUsuario = nomeExibicao,
            Dependentes = alunos.Select(a => new {
                a.Id,
                a.NomeCompleto,
                Turmas = a.Turmas.Where(t => t.Ativo).Select(ta => new {
                    ta.Turma.Id,
                    ta.Turma.Nome,
                    ta.Turma.Nivel,
                    ta.Turma.GradeHorarios,
                    ta.Turma.Sala,
                    Modalidades = ta.Turma.Modalidades.Select(m => m.Nome),
                    Horarios = ta.Turma.Horarios.Select(h => new {
                        h.DiaSemana,
                        h.HoraInicio,
                        h.HoraFim
                    })
                }),
                Faturas = a.Faturas.Select(f => new {
                    f.Id,
                    f.ValorTotal,
                    f.DataVencimento,
                    f.Status
                })
            })
        });
    }
}
