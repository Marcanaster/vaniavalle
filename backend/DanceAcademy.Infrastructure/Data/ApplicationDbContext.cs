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
    public DbSet<AulaExperimental> AulasExperimentais => Set<AulaExperimental>();
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<TurmaAluno> TurmasAlunos => Set<TurmaAluno>();
    public DbSet<FaturaItem> FaturaItems => Set<FaturaItem>();
    public DbSet<Presenca> Presencas => Set<Presenca>();
    public DbSet<TurmaHorario> TurmasHorarios => Set<TurmaHorario>();
    public DbSet<AulaOcorrencia> AulasOcorrencias => Set<AulaOcorrencia>();

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

        // Relacionamento Turma -> Professor
        modelBuilder.Entity<Turma>()
            .HasOne(t => t.Professor)
            .WithMany(p => p.Turmas)
            .HasForeignKey(t => t.ProfessorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relacionamento TurmaAluno (Matrícula)
        modelBuilder.Entity<TurmaAluno>()
            .HasOne(ta => ta.Turma)
            .WithMany(t => t.AlunosMatriculados)
            .HasForeignKey(ta => ta.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TurmaAluno>()
            .HasOne(ta => ta.Aluno)
            .WithMany(a => a.Turmas)
            .HasForeignKey(ta => ta.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento Fatura -> Aluno
        modelBuilder.Entity<Fatura>()
            .HasOne(f => f.Aluno)
            .WithMany(a => a.Faturas)
            .HasForeignKey(f => f.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento Fatura -> FaturaItem
        modelBuilder.Entity<FaturaItem>()
            .HasOne(fi => fi.Fatura)
            .WithMany(f => f.Items)
            .HasForeignKey(fi => fi.FaturaId)
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

        // Relacionamento Presenca
        modelBuilder.Entity<Presenca>()
            .HasOne(p => p.Turma)
            .WithMany()
            .HasForeignKey(p => p.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Presenca>()
            .HasOne(p => p.Aluno)
            .WithMany()
            .HasForeignKey(p => p.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamentos TurmaHorario
        modelBuilder.Entity<TurmaHorario>()
            .HasOne(th => th.Turma)
            .WithMany(t => t.Horarios)
            .HasForeignKey(th => th.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamentos AulaOcorrencia
        modelBuilder.Entity<AulaOcorrencia>()
            .HasOne(ao => ao.Turma)
            .WithMany()
            .HasForeignKey(ao => ao.TurmaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento AgendamentoAula -> AulaOcorrencia
        modelBuilder.Entity<AgendamentoAula>()
            .HasOne(ag => ag.AulaOcorrencia)
            .WithMany(ao => ao.Presencas)
            .HasForeignKey(ag => ag.AulaOcorrenciaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
