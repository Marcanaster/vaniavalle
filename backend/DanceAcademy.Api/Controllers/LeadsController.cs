using DanceAcademy.Api.DTOs;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/leads")]
[Authorize(Roles = "Admin")]
public class LeadsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LeadsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLeads()
    {
        var leads = await _context.AulasExperimentais
            .OrderByDescending(l => l.DataSolicitacao)
            .ToListAsync();
        return Ok(leads);
    }

    [HttpGet("pendentes")]
    public async Task<IActionResult> GetLeadsPendentes()
    {
        var leads = await _context.AulasExperimentais
            .Where(l => l.Status == "Pendente" || l.Status == "Agendada")
            .OrderByDescending(l => l.DataSolicitacao)
            .ToListAsync();
            
        return Ok(leads);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLead([FromBody] LeadCreateDto dto)
    {
        var lead = new AulaExperimental
        {
            Id = Guid.NewGuid(),
            Nome = dto.Nome,
            TelefoneWhatsApp = dto.TelefoneWhatsApp,
            Idade = dto.Idade,
            ModalidadeInteresse = dto.ModalidadeInteresse,
            DataAgendada = dto.DataAgendada,
            ObservacoesAgent = dto.ObservacoesAgent,
            Status = !string.IsNullOrEmpty(dto.Status) ? dto.Status : (dto.DataAgendada.HasValue ? "Agendada" : "Pendente")
        };

        _context.AulasExperimentais.Add(lead);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllLeads), new { id = lead.Id }, lead);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLead(Guid id, [FromBody] LeadCreateDto dto)
    {
        var lead = await _context.AulasExperimentais.FindAsync(id);
        if (lead == null) return NotFound();

        lead.Nome = dto.Nome;
        lead.TelefoneWhatsApp = dto.TelefoneWhatsApp;
        lead.Idade = dto.Idade;
        lead.ModalidadeInteresse = dto.ModalidadeInteresse;
        lead.DataAgendada = dto.DataAgendada;
        lead.ObservacoesAgent = dto.ObservacoesAgent;
        
        if (!string.IsNullOrEmpty(dto.Status))
        {
            lead.Status = dto.Status;
        }
        else if (dto.DataAgendada.HasValue && lead.Status == "Pendente")
        {
            lead.Status = "Agendada";
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLead(Guid id)
    {
        var lead = await _context.AulasExperimentais.FindAsync(id);
        if (lead == null) return NotFound();

        _context.AulasExperimentais.Remove(lead);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
