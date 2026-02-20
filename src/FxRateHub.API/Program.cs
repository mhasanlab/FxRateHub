using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FxRateHub.API.Middleware;
using FxRateHub.Application;
using FxRateHub.Infrastructure;
using FxRateHub.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog Setup
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/fxratehub-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));

// 2. Service Registration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// 3. Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FX Rate Hub API",
        Version = "v1",
        Description = "Open Source FX Rate API Service"
    });

    // Bearer token security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Just enter your token (without 'Bearer ' prefix).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityDefinition("X-API-Key", new OpenApiSecurityScheme
    {
        Description = "API Key authentication header",
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKey"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-API-Key"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FxRateHub";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "FxRateHubUsers";
var jwtExpirationDays = builder.Configuration.GetValue<int>("Jwt:ExpirationDays", 7);

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

// 5. Authorization - AdminOnly policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// 6. CORS - AllowAngular policy
var corsAllowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(corsAllowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// 7. Middleware Pipeline
app.UseSerilogRequestLogging();

// Configure Swagger
app.UseSwagger(c =>
{
    c.RouteTemplate = "api-docs/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api-docs/v1/swagger.json", "FX Rate Hub API v1");
    c.RoutePrefix = "api-docs";
});

// Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Redirect root URL to Swagger UI
app.MapGet("/", () => Results.Redirect("/api-docs/index.html"));

// 8. Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<FxRateHub.Application.Common.Interfaces.IPasswordHasher>();
    var fxRateProvider = scope.ServiceProvider.GetRequiredService<FxRateHub.Application.Common.Interfaces.IFxRateProvider>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();
    
    try
    {
        Log.Information("Applying database migrations...");
        await context.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully.");
        
        Log.Information("Seeding database...");
        await DatabaseSeeder.SeedAsync(context, passwordHasher, logger);
        Log.Information("Database seeding completed.");
        
        // Seed exchange rates from CurrencyFreaks API if not already present
        Log.Information("Performing initial exchange rate sync...");
        await DatabaseSeeder.SeedExchangeRatesAsync(context, fxRateProvider, logger);
        Log.Information("Initial exchange rate sync completed.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while applying migrations or seeding the database.");
    }
}

// 9. Start application with log message
Log.Information("FX Rate Hub API is starting...");
app.Run();
