﻿// File: BankingSystem.API/Program.cs

using BankingSystem.Application.Features.Accounts;
using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
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
using BankingSystem.API.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// ✅ SERILOG CONFIGURATION
// ==========================================================
builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                 .Enrich.WithProperty("Application", "BankingSystemAPI"));

// ==========================================================
// ✅ ADD SERVICES
// ==========================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- DATABASE & REPOSITORIES ---
// Use SQLite for Docker and local development
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BankingSystemDbContext>(options =>
{
    options.UseSqlite(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// Repositories & Services
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IExternalPaymentService, MockExternalPaymentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICacheService, CacheService>();

// MediatR registration (Command/Query handlers)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateAccountCommand).Assembly));

// ==========================================================
// ✅ IDENTITY + JWT AUTH
// ==========================================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BankingSystemDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ??
    throw new InvalidOperationException("JWT Secret not configured."));

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
// ✅ REDIS CACHE
// ==========================================================
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "BankingSystem_";
});

// ==========================================================
// ✅ SWAGGER CONFIGURATION
// ==========================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BankingSystem API",
        Version = "v1",
        Description = "A secure and modular Banking API with JWT Authentication, Caching, and SQLite persistence."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

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
// ✅ BUILD APP
// ==========================================================
var app = builder.Build();

// ==========================================================
// ✅ MIDDLEWARE PIPELINE
// ==========================================================
app.UseSerilogRequestLogging();
app.UseExceptionMiddleware();

// Enable Swagger in all environments for Render deployment
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankingSystem API v1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at root (https://bankingsystem101.onrender.com/)
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ==========================================================
// ✅ HEALTH CHECK & ROOT ENDPOINT
// ==========================================================
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    service = "BankingSystem API",
    version = "v1"
})).AllowAnonymous();

app.MapControllers();

// Global exception fallback (safety net)
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later.");
    }
});

if (!app.Environment.EnvironmentName.Contains("ef", StringComparison.OrdinalIgnoreCase))
{
    app.Run();
}