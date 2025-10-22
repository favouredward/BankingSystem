using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace BankingSystem.Infrastructure.Data
{
    // This factory tells the 'dotnet ef' tool exactly how to instantiate the DbContext.
    public class BankingSystemDbContextFactory : IDesignTimeDbContextFactory<BankingSystemDbContext>
    {
        public BankingSystemDbContext CreateDbContext(string[] args)
        {
            // 1. Build configuration to read the connection string from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Use the startup project directory
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Get the connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // 3. Configure the DbContext
            var builder = new DbContextOptionsBuilder<BankingSystemDbContext>();
            builder.UseSqlServer(connectionString);

            return new BankingSystemDbContext(builder.Options);
        }
    }
}