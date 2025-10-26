using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BankingSystem.Infrastructure.Data
{
    public class BankingSystemDbContextFactory : IDesignTimeDbContextFactory<BankingSystemDbContext>
    {
        public BankingSystemDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(Path.Combine(basePath, "../BankingSystem.API/appsettings.json"), optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<BankingSystemDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new BankingSystemDbContext(optionsBuilder.Options);
        }
    }
}
