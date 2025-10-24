using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BankingSystem.API.Configuration
{
    public static class SerilogConfig
    {
        public static void ConfigureSerilog(IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "BankingSystemAPI")
                    .WriteTo.Console()
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .ReadFrom.Configuration(context.Configuration);
            });
        }
    }
}
