using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FxRateHub.Application.Features.ExchangeRates.Dtos;

namespace FxRateHub.Application.Interfaces;

/// <summary>
/// Service interface for exchange rate operations
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Gets exchange rates with the specified base currency
    /// </summary>
    Task<ExchangeRatesResponseDto> GetExchangeRatesAsync(string baseCurrency);

    /// <summary>
    /// Converts an amount from one currency to another
    /// </summary>
    Task<ConversionResultDto> ConvertCurrencyAsync(string from, string to, decimal amount);

    /// <summary>
    /// Gets list of available currency codes
    /// </summary>
    Task<List<string>> GetAvailableCurrenciesAsync();
}
