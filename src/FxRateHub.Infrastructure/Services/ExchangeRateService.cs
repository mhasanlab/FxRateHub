using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using FxRateHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Infrastructure.Services;

/// <summary>
/// Implementation of exchange rate service operations
/// </summary>
public class ExchangeRateService : IExchangeRateService
{
    private readonly IApplicationDbContext _context;

    public ExchangeRateService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ExchangeRatesResponseDto> GetExchangeRatesAsync(string baseCurrency)
    {
        var baseCurrencyUpper = baseCurrency.ToUpper();
        
        // Get all rates for the base currency
        var rates = await _context.ExchangeRates
            .Where(r => EF.Functions.Like(r.BaseCurrency, baseCurrencyUpper))
            .ToDictionaryAsync(r => r.TargetCurrency, r => r.Rate);
        
        if (!rates.Any())
        {
            // Return empty rates if no data found
            rates = new Dictionary<string, decimal>();
        }
        
        return new ExchangeRatesResponseDto(
            BaseCurrency: baseCurrencyUpper,
            Timestamp: DateTime.UtcNow,
            Rates: rates
        );
    }

    /// <inheritdoc />
    public async Task<ConversionResultDto> ConvertCurrencyAsync(string from, string to, decimal amount)
    {
        var fromCurrency = from.ToUpper();
        var toCurrency = to.ToUpper();
        
        // If same currency, return direct conversion
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return new ConversionResultDto(
                From: fromCurrency,
                To: toCurrency,
                Amount: amount,
                ConvertedAmount: amount,
                Rate: 1,
                Timestamp: DateTime.UtcNow
            );
        }
        
        // Try to find direct rate
        var directRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => 
                EF.Functions.Like(r.BaseCurrency, fromCurrency) &&
                EF.Functions.Like(r.TargetCurrency, toCurrency));
        
        if (directRate != null)
        {
            var convertedAmount = amount * directRate.Rate;
            return new ConversionResultDto(
                From: fromCurrency,
                To: toCurrency,
                Amount: amount,
                ConvertedAmount: convertedAmount,
                Rate: directRate.Rate,
                Timestamp: directRate.UpdatedAt
            );
        }
        
        // Try reverse rate
        var reverseRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => 
                EF.Functions.Like(r.BaseCurrency, toCurrency) &&
                EF.Functions.Like(r.TargetCurrency, fromCurrency));
        
        if (reverseRate != null)
        {
            var convertedAmount = amount / reverseRate.Rate;
            return new ConversionResultDto(
                From: fromCurrency,
                To: toCurrency,
                Amount: amount,
                ConvertedAmount: convertedAmount,
                Rate: 1 / reverseRate.Rate,
                Timestamp: reverseRate.UpdatedAt
            );
        }
        
        // If no direct or reverse rate, try to convert via USD
        if (!fromCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase) &&
            !toCurrency.Equals("USD", StringComparison.OrdinalIgnoreCase))
        {
            // Get from -> USD rate
            var fromToUsd = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => 
                    EF.Functions.Like(r.BaseCurrency, fromCurrency) &&
                    EF.Functions.Like(r.TargetCurrency, "USD"));
            
            var usdToFrom = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => 
                    EF.Functions.Like(r.BaseCurrency, "USD") &&
                    EF.Functions.Like(r.TargetCurrency, fromCurrency));
            
            // Get USD -> to rate
            var usdToTo = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => 
                    EF.Functions.Like(r.BaseCurrency, "USD") &&
                    EF.Functions.Like(r.TargetCurrency, toCurrency));
            
            var toToUsd = await _context.ExchangeRates
                .FirstOrDefaultAsync(r => 
                    EF.Functions.Like(r.BaseCurrency, toCurrency) &&
                    EF.Functions.Like(r.TargetCurrency, "USD"));
            
            decimal? fromToUsdRate = null;
            DateTime? timestamp = null;
            
            if (fromToUsd != null)
            {
                fromToUsdRate = fromToUsd.Rate;
                timestamp = fromToUsd.UpdatedAt;
            }
            else if (usdToFrom != null)
            {
                fromToUsdRate = 1 / usdToFrom.Rate;
                timestamp = usdToFrom.UpdatedAt;
            }
            
            decimal? usdToToRate = null;
            if (usdToTo != null)
            {
                usdToToRate = usdToTo.Rate;
                timestamp = usdToTo.UpdatedAt;
            }
            else if (toToUsd != null)
            {
                usdToToRate = 1 / toToUsd.Rate;
                timestamp = toToUsd.UpdatedAt;
            }
            
            if (fromToUsdRate.HasValue && usdToToRate.HasValue)
            {
                var effectiveRate = fromToUsdRate.Value * usdToToRate.Value;
                var convertedAmount = amount * effectiveRate;
                return new ConversionResultDto(
                    From: fromCurrency,
                    To: toCurrency,
                    Amount: amount,
                    ConvertedAmount: convertedAmount,
                    Rate: effectiveRate,
                    Timestamp: timestamp ?? DateTime.UtcNow
                );
            }
        }
        
        // Return a result indicating conversion not possible
        throw new InvalidOperationException($"Cannot convert from '{fromCurrency}' to '{toCurrency}': exchange rate not found");
    }

    /// <inheritdoc />
    public async Task<List<string>> GetAvailableCurrenciesAsync()
    {
        var baseCurrencies = await _context.ExchangeRates
            .Select(r => r.BaseCurrency)
            .Distinct()
            .ToListAsync();
        
        var targetCurrencies = await _context.ExchangeRates
            .Select(r => r.TargetCurrency)
            .Distinct()
            .ToListAsync();
        
        return baseCurrencies.Union(targetCurrencies).OrderBy(c => c).ToList();
    }
}
