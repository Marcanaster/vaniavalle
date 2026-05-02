using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DanceAcademy.Infrastructure.Data;
using DanceAcademy.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("d:/VaniaValle/backend/DanceAcademy.Api/appsettings.json", optional: false, reloadOnChange: true);
});

builder.ConfigureServices((hostContext, services) =>
{
    var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
});

using var host = builder.Build();
using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

Console.WriteLine("Buscando duplicatas em Responsaveis...");

var duplicados = context.Responsaveis
    .GroupBy(r => r.Documento)
    .Where(g => g.Count() > 1 && !string.IsNullOrWhiteSpace(g.Key))
    .Select(g => new { Documento = g.Key, Ids = g.Select(r => r.Id).ToList() })
    .ToList();

if (!duplicados.Any())
{
    Console.WriteLine("Nenhuma duplicata de Documento encontrada.");
}
else
{
    foreach (var dup in duplicados)
    {
        Console.WriteLine($"Documento {dup.Documento} tem {dup.Ids.Count} registros.");
        var principalId = dup.Ids.First();
        var secundários = dup.Ids.Skip(1).ToList();

        foreach (var secId in secundários)
        {
            var alunos = context.Alunos.Where(a => a.ResponsavelId == secId).ToList();
            foreach (var aluno in alunos)
            {
                aluno.ResponsavelId = principalId;
            }
            var respRemover = context.Responsaveis.Find(secId);
            if (respRemover != null) context.Responsaveis.Remove(respRemover);
        }
    }
    context.SaveChanges();
    Console.WriteLine("Duplicatas de Documento resolvidas.");
}

// Repetir para Email
var duplicadosEmail = context.Responsaveis
    .GroupBy(r => r.Email)
    .Where(g => g.Count() > 1 && !string.IsNullOrWhiteSpace(g.Key))
    .Select(g => new { Email = g.Key, Ids = g.Select(r => r.Id).ToList() })
    .ToList();

if (!duplicadosEmail.Any())
{
    Console.WriteLine("Nenhuma duplicata de Email encontrada.");
}
else
{
    foreach (var dup in duplicadosEmail)
    {
        Console.WriteLine($"Email {dup.Email} tem {dup.Ids.Count} registros.");
        var principalId = dup.Ids.First();
        var secundários = dup.Ids.Skip(1).ToList();

        foreach (var secId in secundários)
        {
            var alunos = context.Alunos.Where(a => a.ResponsavelId == secId).ToList();
            foreach (var aluno in alunos)
            {
                aluno.ResponsavelId = principalId;
            }
            var respRemover = context.Responsaveis.Find(secId);
            if (respRemover != null) context.Responsaveis.Remove(respRemover);
        }
    }
    context.SaveChanges();
    Console.WriteLine("Duplicatas de Email resolvidas.");
}
