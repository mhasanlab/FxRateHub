using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FxRateHub.Infrastructure.Persistence;
using FxRateHub.Infrastructure.Services;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Infrastructure.ExternalServices;
using FxRateHub.Infrastructure.BackgroundJobs;
using Quartz;
using Quartz.Impl;

namespace FxRateHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext Registration
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
        });

        // IApplicationDbContext Registration
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Service Registrations
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        // HttpClient for CurrencyFreaks
        services.AddHttpClient<IFxRateProvider, CurrencyFreaksService>();

        // Quartz Jobs Configuration
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.AddQuartz(q =>
        {
            // FxRateSyncJob - runs every hour at minute 0
            var jobKey = new JobKey("FxRateSyncJob");
            q.AddJob<FxRateSyncJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithCronSchedule("0 0 * * * ?"));

            // DailyResetJob - runs at midnight UTC daily
            var dailyResetJobKey = new JobKey("DailyResetJob");
            q.AddJob<DailyResetJob>(opts => opts.WithIdentity(dailyResetJobKey));
            q.AddTrigger(opts => opts
                .ForJob(dailyResetJobKey)
                .WithCronSchedule("0 0 0 * * ?"));
        });

        return services;
    }
}
