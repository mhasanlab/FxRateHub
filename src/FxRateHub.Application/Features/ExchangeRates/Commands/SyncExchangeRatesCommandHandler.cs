using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using FxRateHub.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FxRateHub.Application.Features.ExchangeRates.Commands;

public class SyncExchangeRatesCommandHandler : IRequestHandler<SyncExchangeRatesCommand, SyncResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IFxRateProvider _fxRateProvider;
    private readonly ILogger<SyncExchangeRatesCommandHandler> _logger;

    // Reasonable rate limits to filter out extreme/unusual values
    private const decimal MaxReasonableRate = 1_000_000_000m;  // 1 billion
    private const decimal MinReasonableRate = 0.00000001m;

    public SyncExchangeRatesCommandHandler(
        IApplicationDbContext context,
        IFxRateProvider fxRateProvider,
        ILogger<SyncExchangeRatesCommandHandler> logger)
    {
        _context = context;
        _fxRateProvider = fxRateProvider;
        _logger = logger;
    }

    public async Task<SyncResultDto> Handle(SyncExchangeRatesCommand request, CancellationToken cancellationToken)
    {
        var syncLog = SyncLog.StartNew();

        try
        {
            // Step 2: Fetch rates from provider
            _logger.LogInformation("Fetching latest exchange rates from provider for base currency USD");
            var rates = await _fxRateProvider.GetLatestRatesAsync("USD", cancellationToken);
            _logger.LogInformation("Fetched {Count} exchange rates from provider", rates.Count);

            // Step 3: Archive existing rates to history
            _logger.LogInformation("Archiving existing exchange rates to history");
            var existingRates = await _context.ExchangeRates.ToListAsync(cancellationToken);
            var historyRecords = existingRates.Select(r => r.ToHistory()).ToList();
            _context.ExchangeRateHistory.AddRange(historyRecords);
            _logger.LogInformation("Archived {Count} exchange rate records to history", historyRecords.Count);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 4: Update ExchangeRates table
            _logger.LogInformation("Updating exchange rates table with new rates");
            var ratesToAdd = new List<ExchangeRate>();

            foreach (var rate in rates)
            {
                var currency = rate.Key;
                var rateValue = rate.Value;

                // Skip extreme/unusual rates (e.g., > 1 billion or < 0.00000001)
                if (rateValue > MaxReasonableRate || rateValue < MinReasonableRate)
                {
                    _logger.LogWarning("Skipping extreme rate for {Currency}: {Rate}", currency, rateValue);
                    continue;
                }

                var existingRate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(r => r.TargetCurrency == currency, cancellationToken);

                if (existingRate != null)
                {
                    // Update existing rate
                    existingRate.Update(rateValue);
                }
                else
                {
                    // Create new rate
                    var newRate = ExchangeRate.Create("USD", currency, rateValue);
                    ratesToAdd.Add(newRate);
                }
            }

            if (ratesToAdd.Any())
            {
                _context.ExchangeRates.AddRange(ratesToAdd);
                _logger.LogInformation("Adding {Count} new exchange rates", ratesToAdd.Count);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully synced {Count} exchange rates", rates.Count);

            // Step 5: Update SyncLog
            syncLog.Complete(rates.Count);
            _context.SyncLogs.Add(syncLog);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 6: Return SyncResultDto
            return new SyncResultDto(
                CurrenciesUpdated: rates.Count,
                LastSyncTime: DateTime.UtcNow,
                Success: true,
                ErrorMessage: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while syncing exchange rates");

            // Update SyncLog with failure
            syncLog.Fail(ex.Message);
            _context.SyncLogs.Add(syncLog);
            await _context.SaveChangesAsync(cancellationToken);

            // Return failed SyncResultDto
            return new SyncResultDto(
                CurrenciesUpdated: 0,
                LastSyncTime: DateTime.UtcNow,
                Success: false,
                ErrorMessage: ex.Message
            );
        }
    }
}
