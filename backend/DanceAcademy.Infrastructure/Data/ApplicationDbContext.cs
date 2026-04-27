using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DanceAcademy.Domain.Entities;

namespace DanceAcademy.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Responsavel> Responsaveis => Set<Responsavel>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Modalidade> Modalidades => Set<Modalidade>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Plano> Planos => Set<Plano>();
    public DbSet<Fatura> Faturas => Set<Fatura>();
    public DbSet<AgendamentoAula> Agendamentos => Set<AgendamentoAula>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Necessário para o Identity

        // Filtro Global para Exclusão Lógica (Soft Delete)
        modelBuilder.Entity<Aluno>().HasQueryFilter(a => a.Ativo);

        // Relacionamento Aluno -> Responsavel
        modelBuilder.Entity<Aluno>()
            .HasOne(a => a.Responsavel)
            .WithMany(r => r.Alunos)
            .HasForeignKey(a => a.ResponsavelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Aluno -> Plano
        modelBuilder.Entity<Aluno>()
            .HasOne(a => a.Plano)
            .WithMany(p => p.Alunos)
            .HasForeignKey(a => a.PlanoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Turma -> Modalidade
        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Modalidade)
            .WithMany(m => m.Turmas)
            .HasForeignKey(t => t.ModalidadeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Fatura -> Aluno
        modelBuilder.Entity<Fatura>()
            .HasOne(f => f.Aluno)
            .WithMany(a => a.Faturas)
            .HasForeignKey(f => f.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento Agendamento -> Aluno e Turma
        modelBuilder.Entity<AgendamentoAula>()
            .HasOne(ag => ag.Aluno)
            .WithMany(a => a.Agendamentos)
            .HasForeignKey(ag => ag.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Constraints de tamanho (Exemplos)
        modelBuilder.Entity<Responsavel>().Property(r => r.Nome).HasMaxLength(150);
        modelBuilder.Entity<Aluno>().Property(a => a.NomeCompleto).HasMaxLength(150);
        modelBuilder.Entity<Turma>().Property(t => t.Nome).HasMaxLength(100);
    }
}
