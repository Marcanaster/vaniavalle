using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Domain.Interfaces;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/professores")]
[Authorize]
public class ProfessoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailService _emailService;

    public ProfessoresController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var professores = await _context.Professores
            .Where(p => p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
            
        return Ok(professores);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var professor = await _context.Professores
            .Include(p => p.Turmas)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);
            
        if (professor == null) return NotFound();
        return Ok(professor);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ProfessorDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var professor = new Professor
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Especialidade = dto.Especialidade,
                Ativo = true
            };

            // Criar o usuário para login do professor
            if (!string.IsNullOrWhiteSpace(professor.Email))
            {
                var user = new IdentityUser { UserName = professor.Email, Email = professor.Email };
                var result = await _userManager.CreateAsync(user, "Prof123$"); // Senha padrão para MVP
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    professor.UserId = user.Id;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest(new { error = $"Erro ao criar usuário de acesso: {errors}" });
                }
            }

            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Enviar email de boas-vindas
            if (!string.IsNullOrWhiteSpace(professor.Email))
            {
                string htmlContent = $@"
                    <h2>Olá, {professor.Nome}! Bem-vindo(a) à Dance Academy Vania Valle.</h2>
                    <p>Seu acesso ao Portal do Professor foi criado com sucesso.</p>
                    <p><b>Seu Login (E-mail):</b> {professor.Email}</p>
                    <p><b>Sua Senha Provisória:</b> Prof123$</p>
                    <br>
                    <p>Acesse o sistema e não se esqueça de alterar sua senha!</p>
                ";
                try {
                    await _emailService.SendEmailAsync(professor.Email, "Bem-vindo à Vania Valle - Acesso ao Portal", htmlContent);
                } catch { /* Ignora erro de email para não travar o cadastro */ }
            }

            return CreatedAtAction(nameof(GetById), new { id = professor.Id }, professor);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProfessorDto dto)
    {
        var professor = await _context.Professores.FindAsync(id);
        if (professor == null || !professor.Ativo) return NotFound();

        professor.Nome = dto.Nome;
        professor.Telefone = dto.Telefone;
        professor.Email = dto.Email;
        professor.Especialidade = dto.Especialidade;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var professor = await _context.Professores.FindAsync(id);
        if (professor == null) return NotFound();

        professor.Ativo = false; // Soft delete
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // --- PORTAL DO PROFESSOR ---

    [HttpGet("minhas-turmas")]
    [Authorize]
    public async Task<IActionResult> GetMinhasTurmas()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");
        
        var professor = await _context.Professores
            .FirstOrDefaultAsync(p => p.UserId == userId);

        // Se for admin e não for professor, para teste vamos pegar as turmas do primeiro professor ativo
        if (professor == null && isAdmin)
        {
            var firstProf = await _context.Professores.FirstOrDefaultAsync(p => p.Ativo);
            if (firstProf != null) professor = firstProf;
        }

        if (professor == null) return NotFound("Perfil de professor não encontrado para este usuário.");

        var turmas = await _context.Turmas
            .Include(t => t.Modalidade)
            .Include(t => t.AlunosMatriculados)
                .ThenInclude(ta => ta.Aluno)
            .Where(t => t.ProfessorId == professor.Id)
            .ToListAsync();

        return Ok(turmas);
    }

    [HttpPost("chamada")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> RegistrarPresenca([FromBody] List<PresencaRequestDto> chamada)
    {
        if (chamada == null || !chamada.Any()) return BadRequest("Dados da chamada vazios.");

        foreach (var item in chamada)
        {
            // Verifica se já existe registro para este aluno nesta turma hoje
            var hoje = DateTime.UtcNow.Date;
            var presencaExistente = await _context.Presencas
                .FirstOrDefaultAsync(p => p.TurmaId == item.TurmaId && p.AlunoId == item.AlunoId && p.Data.Date == hoje);

            if (presencaExistente != null)
            {
                presencaExistente.Presente = item.Presente;
                presencaExistente.Observacao = item.Observacao;
            }
            else
            {
                _context.Presencas.Add(new Presenca
                {
                    Id = Guid.NewGuid(),
                    TurmaId = item.TurmaId,
                    AlunoId = item.AlunoId,
                    Data = DateTime.UtcNow,
                    Presente = item.Presente,
                    Observacao = item.Observacao
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Chamada registrada com sucesso." });
    }
}

public class PresencaRequestDto
{
    public Guid TurmaId { get; set; }
    public Guid AlunoId { get; set; }
    public bool Presente { get; set; }
    public string? Observacao { get; set; }
}
