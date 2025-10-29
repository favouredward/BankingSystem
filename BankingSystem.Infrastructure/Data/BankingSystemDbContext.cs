using BankingSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Data
{
   
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
            
            modelBuilder.Entity<Account>()
                .ToTable("Accounts")
                .HasMany(a => a.Transactions)
                .WithOne()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.NewBalance)
                .HasPrecision(18, 2);

            
            base.OnModelCreating(modelBuilder);
        }
    }
}
