using System;
using System.Threading.Tasks;
using System.Linq;
using Quartz;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Infrastructure.Persistence;

namespace FxRateHub.Infrastructure.BackgroundJobs;

/// <summary>
/// A Quartz job that resets the daily request count for all API keys.
/// This job runs daily to ensure API usage tracking starts fresh each day.
/// </summary>
[DisallowConcurrentExecution]
public class DailyResetJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyResetJob> _logger;

    public DailyResetJob(IServiceProvider serviceProvider, ILogger<DailyResetJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var timestamp = DateTime.UtcNow;
        _logger.LogInformation("DailyResetJob started at {Timestamp}", timestamp);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var apiKeys = await dbContext.ApiKeys.ToListAsync();

            foreach (var apiKey in apiKeys)
            {
                apiKey.DailyRequestCount = 0;
                apiKey.LastRequestDate = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("DailyResetJob completed. {Count} API keys were reset", apiKeys.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DailyResetJob encountered an error");
            throw;
        }
    }
}
