using System.Security.Claims;
using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Domain.Interfaces;
using DanceAcademy.Infrastructure.Data;
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
                Ativo = true
            };

            // Lógica de Responsável e Acesso
            Responsavel? responsavel = null;
            string? userId = null;

            if (dto.Responsavel != null && !string.IsNullOrWhiteSpace(dto.Responsavel.Email))
            {
                // Tenta encontrar responsável existente pelo e-mail
                responsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => r.Email == dto.Responsavel.Email);

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
                
                aluno.ResponsavelId = responsavel.Id;
            }

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Gerenciar Usuário de Acesso
            string emailLogin = responsavel?.Email ?? (string.IsNullOrWhiteSpace(aluno.Cpf) ? $"aluno.{aluno.Id.ToString().Substring(0,8)}@danceacademy.com" : $"aluno.{aluno.Cpf}@danceacademy.com");
            
            var existingUser = await _userManager.FindByEmailAsync(emailLogin);
            if (existingUser == null)
            {
                var user = new IdentityUser { UserName = emailLogin, Email = emailLogin };
                var result = await _userManager.CreateAsync(user, "Aluno123$");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    userId = user.Id;
                    if (responsavel != null)
                    {
                        responsavel.UserId = userId;
                    }
                    else
                    {
                        aluno.UserId = userId;
                    }
                }
            }
            else
            {
                userId = existingUser.Id;
                if (responsavel != null && responsavel.UserId == null)
                {
                    responsavel.UserId = userId;
                }
                else if (responsavel == null && aluno.UserId == null)
                {
                    aluno.UserId = userId;
                }
            }

            await _context.SaveChangesAsync();

            // E-mail de Boas-vindas
            if (userId != null)
            {
                string destinatario = responsavel?.Nome ?? aluno.NomeCompleto;
                string htmlContent = $@"
                    <h2>Olá, {destinatario}!</h2>
                    <p>A matrícula de <b>{aluno.NomeCompleto}</b> na Dance Academy Vania Valle foi concluída com sucesso.</p>
                    <p>Acesse o Portal do Aluno para acompanhar as aulas e faturas:</p>
                    <p><b>Login:</b> {emailLogin}</p>
                    <p><b>Senha Provisória:</b> Aluno123$</p>
                    <br>
                    <p>Se você já possui outros alunos cadastrados, utilize seu login atual.</p>
                ";
                try { await _emailService.SendEmailAsync(emailLogin, "Matrícula Concluída - Dance Academy", htmlContent); }
                catch { /* Log error */ }
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
            if (aluno.Responsavel != null)
            {
                aluno.Responsavel.Nome = dto.Responsavel.Nome;
                aluno.Responsavel.Documento = dto.Responsavel.Documento;
                aluno.Responsavel.Email = dto.Responsavel.Email;
                aluno.Responsavel.Telefone = dto.Responsavel.Telefone;
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
                .ThenInclude(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Modalidade)
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
                .Include(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Modalidade)
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
                    .Include(a => a.Turmas).ThenInclude(ta => ta.Turma).ThenInclude(t => t.Modalidade)
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
                    Modalidade = ta.Turma.Modalidade.Nome,
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
