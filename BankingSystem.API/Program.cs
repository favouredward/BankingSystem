// File: BankingSystem.API/Program.cs

using BankingSystem.Application.Features.Accounts;
using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities; // <-- NEW: Required for ApplicationUser
using BankingSystem.Infrastructure.Data;
using BankingSystem.Infrastructure.Repositories;
using BankingSystem.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using BankingSystem.API.Middleware; // Required for context.Response.WriteAsync

// ==========================================================
// ✅ CONFIGURE SERILOG
// ==========================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                 .Enrich.WithProperty("Application", "BankingSystemAPI"));

// ==========================================================
// ✅ ADD SERVICES TO THE CONTAINER
// ==========================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- DATABASE & REPOSITORIES ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BankingSystemDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repositories and Services
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IExternalPaymentService, MockExternalPaymentService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Used for JWT generation/validation

// Register MediatR: Automatically scans the assembly where CreateAccountCommand lives
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly));

// ==========================================================
// ✅ IDENTITY & AUTHENTICATION (JWT)
// ==========================================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BankingSystemDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured."));

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

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ==========================================================
// ✅ CACHING & SWAGGER CONFIGURATION
// ==========================================================
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "BankingSystem_";
});

builder.Services.AddScoped<ICacheService, CacheService>(); // ✅ register cache abstraction

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BankingSystem API", Version = "v1" });

    // Define security scheme (Bearer token)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // Apply security requirement globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================================================
// ✅ BUILD APPLICATION
// ==========================================================
var app = builder.Build();

// ==========================================================
// ✅ CONFIGURE MIDDLEWARE PIPELINE
// ==========================================================

// Enable request logging via Serilog
app.UseSerilogRequestLogging();

// Use custom global exception middleware
app.UseExceptionMiddleware();

// Configure Swagger for dev/testing environments
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controller Endpoints
app.MapControllers();

// ==========================================================
// ✅ Fallback Exception Handler (Safety Net)
// ==========================================================
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An unhandled exception occurred.");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
    }
});

// ==========================================================
// ✅ CLEAN RUN LOGIC
// ==========================================================
if (!app.Environment.EnvironmentName.Contains("ef", StringComparison.OrdinalIgnoreCase))
{
    app.Run();
}
