// File: BankingSystem.Infrastructure/Data/BankingSystemDbContext.cs

using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Data
{
    /// <summary>
    /// Main EF Core database context for the Banking System.
    /// Uses SQLite for persistence and integrates with ASP.NET Identity.
    /// </summary>
    public class BankingSystemDbContext : IdentityDbContext
    {
        public BankingSystemDbContext(DbContextOptions<BankingSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Account–Transaction relationship
            modelBuilder.Entity<Account>()
                .ToTable("Accounts")
                .HasMany(a => a.Transactions)
                .WithOne()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.NewBalance)
                .HasPrecision(18, 2);

            // Important: Keep this for Identity tables
            base.OnModelCreating(modelBuilder);
        }
    }
}
