using DanceAcademy.Domain.Entities;
using DanceAcademy.Domain.Helpers;
using DanceAcademy.Domain.Interfaces;
using DanceAcademy.Infrastructure.Data;
using DanceAcademy.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        ApplicationDbContext context, 
        UserManager<IdentityUser> userManager, 
        IEmailService emailService, 
        ILogger<AdminController> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("emergency-bulk-notify")]
    public async Task<IActionResult> EmergencyBulkNotify()
    {
        int successCount = 0;
        int errorCount = 0;

        // 1. Processar Responsáveis
        var responsaveis = await _context.Responsaveis.ToListAsync();
        foreach (var responsavel in responsaveis)
        {
            if (string.IsNullOrWhiteSpace(responsavel.Email)) continue;

            try
            {
                var user = await _userManager.FindByEmailAsync(responsavel.Email);
                string newPassword = PasswordHelper.GenerateRandomPassword();

                if (user == null)
                {
                    user = new IdentityUser { UserName = responsavel.Email, Email = responsavel.Email };
                    var createResult = await _userManager.CreateAsync(user, newPassword);
                    if (!createResult.Succeeded) throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    
                    await _userManager.AddToRoleAsync(user, "Student");
                    responsavel.UserId = user.Id;
                }
                else
                {
                    // Resetar senha
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, newPassword);
                    
                    if (responsavel.UserId == null) responsavel.UserId = user.Id;
                    
                    if (!await _userManager.IsInRoleAsync(user, "Student"))
                    {
                        await _userManager.AddToRoleAsync(user, "Student");
                    }
                }

                // Enviar Email
                string htmlContent = EmailTemplates.GetWelcomeTemplate(responsavel.Nome, responsavel.Email, newPassword, "Student");
                await _emailService.SendEmailAsync(responsavel.Email, "Acesso ao Portal - Dance Academy Vania Valle", htmlContent);
                
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar responsável {Email}", responsavel.Email);
                errorCount++;
            }
        }

        // 2. Processar Professores
        var professores = await _context.Professores.Where(p => p.Ativo).ToListAsync();
        foreach (var professor in professores)
        {
            if (string.IsNullOrWhiteSpace(professor.Email)) continue;

            try
            {
                var user = await _userManager.FindByEmailAsync(professor.Email);
                string newPassword = PasswordHelper.GenerateRandomPassword();

                if (user == null)
                {
                    user = new IdentityUser { UserName = professor.Email, Email = professor.Email };
                    var createResult = await _userManager.CreateAsync(user, newPassword);
                    if (!createResult.Succeeded) throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    professor.UserId = user.Id;
                }
                else
                {
                    // Resetar senha
                    await _userManager.RemovePasswordAsync(user);
                    await _userManager.AddPasswordAsync(user, newPassword);
                    
                    if (professor.UserId == null) professor.UserId = user.Id;
                    
                    if (!await _userManager.IsInRoleAsync(user, "Teacher"))
                    {
                        await _userManager.AddToRoleAsync(user, "Teacher");
                    }
                }

                // Enviar Email
                string htmlContent = EmailTemplates.GetWelcomeTemplate(professor.Nome, professor.Email, newPassword, "Teacher");
                await _emailService.SendEmailAsync(professor.Email, "Acesso ao Portal - Dance Academy Vania Valle", htmlContent);
                
                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar professor {Email}", professor.Email);
                errorCount++;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { 
            Message = "Processo de notificação concluído.",
            SuccessCount = successCount,
            ErrorCount = errorCount
        });
    }
}
