using System;
using System.Threading.Tasks;
using Quartz;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FxRateHub.Application.Features.ExchangeRates.Commands;

namespace FxRateHub.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class FxRateSyncJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FxRateSyncJob> _logger;

    public FxRateSyncJob(IServiceProvider serviceProvider, ILogger<FxRateSyncJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var timestamp = DateTime.UtcNow;
        _logger.LogInformation("FxRateSyncJob started at {Timestamp}", timestamp);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new SyncExchangeRatesCommand());

            if (result.Success)
            {
                _logger.LogInformation("FxRateSyncJob completed. {Count} currencies updated", result.CurrenciesUpdated);
            }
            else
            {
                _logger.LogWarning("FxRateSyncJob failed: {Error}", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FxRateSyncJob encountered an error");
            throw;
        }
    }
}
