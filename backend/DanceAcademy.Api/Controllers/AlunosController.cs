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

            if (dto.Responsavel != null && !string.IsNullOrWhiteSpace(dto.Responsavel.Nome))
            {
                var responsavel = new Responsavel
                {
                    Id = Guid.NewGuid(),
                    Nome = dto.Responsavel.Nome,
                    Documento = dto.Responsavel.Documento,
                    Email = dto.Responsavel.Email,
                    Telefone = dto.Responsavel.Telefone
                };
                _context.Responsaveis.Add(responsavel);
                aluno.ResponsavelId = responsavel.Id;
            }

            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();

            // Criar o usuário de acesso para o aluno (Identity)
            string emailLogin = dto.Responsavel?.Email ?? $"aluno.{aluno.Cpf}@danceacademy.com";
            var user = new IdentityUser { UserName = emailLogin, Email = emailLogin };
            var result = await _userManager.CreateAsync(user, "Aluno123$"); // Senha padrão para MVP
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
                
                // ------------------------------------------

                string htmlContent = $@"
                    <h2>Olá, {aluno.NomeCompleto}! Bem-vindo(a) à Dance Academy Vania Valle.</h2>
                    <p>Sua matrícula foi concluída. Abaixo estão os seus dados de acesso ao Portal do Aluno:</p>
                    <p><b>Seu Login (E-mail):</b> {emailLogin}</p>
                    <p><b>Sua Senha Provisória:</b> Aluno123$</p>
                    <br>
                    <p>Acesse o sistema para verificar sua agenda e faturas!</p>
                ";
                try
                {
                    await _emailService.SendEmailAsync(emailLogin, "Bem-vindo à Vania Valle - Dados de Acesso", htmlContent);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao enviar e-mail de boas-vindas para {Email}, mas o cadastro foi concluído.", emailLogin);
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
}
