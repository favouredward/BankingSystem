using BankingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace BankingSystem.Infrastructure.Data
{
    // Inheriting from IdentityDbContext provides all the AspNetIdentity tables automatically.
    public class BankingSystemDbContext : IdentityDbContext
    {
        public BankingSystemDbContext(DbContextOptions<BankingSystemDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationship between Account and Transaction.
            modelBuilder.Entity<Account>()
                .ToTable("Accounts")
                .HasMany(a => a.Transactions)
                .WithOne()
                .HasForeignKey(t => t.AccountId);

            // Define precision for decimal properties to prevent truncation and rounding errors
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.NewBalance)
                .HasPrecision(18, 2);

            // Calling base.OnModelCreating is essential for IdentityDbContext to configure Identity tables
            base.OnModelCreating(modelBuilder);
        }
    }
}
