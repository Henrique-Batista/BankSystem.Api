using BankSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Data;

public sealed class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
    {
    }
    
    public DbSet<Conta> Contas { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Cliente> Clientes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .HasMany(c => c.Contas)
            .WithOne( a => a.Cliente)
            .HasForeignKey(f => f.Cliente_Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cliente>()
            .Property(a => a.Cpf)
            .HasConversion<string>();

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Cpf)
            .IsUnique();
        
        modelBuilder.Entity<Conta>()
            .Property(c => c.Saldo)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Conta>()
            .Property(c => c.Numero)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Transacao>()
            .Property(t => t.Valor)
            .HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.ContaOrigem)
            .WithMany(c => c.TransacoesComoOrigem)
            .HasForeignKey(t => t.Conta_Origem_Id)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.ContaDestino)
            .WithMany(c => c.TransacoesComoDestino)
            .HasForeignKey(t => t.Conta_Destino_Id)
            .OnDelete(DeleteBehavior.NoAction);
    }
}