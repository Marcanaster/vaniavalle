using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using DanceAcademy.Domain.Interfaces;
using DanceAcademy.Infrastructure.Services;
using DanceAcademy.Api.Services;
using Resend;
using DanceAcademy.Domain.Entities;
using DanceAcademy.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configuração para permitir DateTime Unspecified no PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found.");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Resend
builder.Services.AddOptions<ResendClientOptions>().Configure(options =>
{
    options.ApiToken = builder.Configuration["Resend:ApiKey"]!;
});
builder.Services.AddHttpClient<ResendClient>();
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();
builder.Services.AddHostedService<FinanceiroBackgroundService>();

var app = builder.Build();

// Auto-Migration e Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        int retries = 5;
        while (retries > 0)
        {
            try
            {
                context.Database.Migrate();
                break;
            }
            catch (Exception)
            {
                retries--;
                if (retries == 0) throw;
                System.Threading.Thread.Sleep(2000);
            }
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Admin", "Student", "Teacher" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminUser = await userManager.FindByEmailAsync("admin@danceacademy.com");
        if (adminUser == null)
        {
            var user = new IdentityUser { UserName = "admin@danceacademy.com", Email = "admin@danceacademy.com" };
            var result = await userManager.CreateAsync(user, "Admin123$");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        // Seed Modalidades Iniciais
        if (!await context.Modalidades.AnyAsync())
        {
            context.Modalidades.AddRange(new List<Modalidade>
            {
                new Modalidade { Id = Guid.NewGuid(), Nome = "Ballet", Descricao = "Dança clássica" },
                new Modalidade { Id = Guid.NewGuid(), Nome = "Jazz", Descricao = "Dança moderna" },
                new Modalidade { Id = Guid.NewGuid(), Nome = "Hip Hop", Descricao = "Dança urbana" }
            });
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrations ou popular os dados iniciais.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar CORS para o Frontend
app.UseCors(builder => builder
    .SetIsOriginAllowed(origin => true) // Para o MVP, permitindo tudo (em prod deve ser restrito)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// Configurar cabeçalhos para o proxy reverso da VPS (Traefik/Nginx)
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeadersOptions.KnownNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

// app.UseHttpsRedirection(); // Removido para funcionar corretamente atrás do Proxy Reverso da Hostinger

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/test/email", async (IEmailService emailService) =>
{
    await emailService.SendEmailAsync(
        "marcanaster@gmail.com",
        "Teste de Integração - Resend",
        "<h1>Deu certo!</h1><p>A integração com o Resend na Academia Vania Valle está funcionando perfeitamente.</p>"
    );
    return Results.Ok(new { Message = "Email enviado com sucesso para marcanaster@gmail.com" });
})
.WithName("TestEmail")
.WithOpenApi();

app.MapControllers();

app.Run();
