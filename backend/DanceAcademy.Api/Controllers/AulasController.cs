using DanceAcademy.Domain.Entities;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DanceAcademy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AulasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AulasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("cancelar")]
    public async Task<IActionResult> CancelarAula([FromBody] CancelamentoAulaDto dto)
    {
        // Se a ocorrência ainda não existe no banco, precisamos criá-la como "Cancelada"
        var ocorrencia = await _context.AulasOcorrencias
            .FirstOrDefaultAsync(o => o.TurmaId == dto.TurmaId && o.DataHora == dto.DataHora);

        if (ocorrencia == null)
        {
            ocorrencia = new AulaOcorrencia
            {
                Id = Guid.NewGuid(),
                TurmaId = dto.TurmaId,
                DataHora = dto.DataHora,
                Status = "Cancelada",
                MotivoCancelamento = dto.Motivo
            };
            _context.AulasOcorrencias.Add(ocorrencia);
        }
        else
        {
            ocorrencia.Status = "Cancelada";
            ocorrencia.MotivoCancelamento = dto.Motivo;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Aula cancelada com sucesso." });
    }
}

public class CancelamentoAulaDto
{
    public Guid TurmaId { get; set; }
    public DateTime DataHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
}
