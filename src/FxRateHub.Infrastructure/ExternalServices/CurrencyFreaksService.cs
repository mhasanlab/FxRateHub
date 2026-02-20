using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FxRateHub.Infrastructure.ExternalServices;

/// <summary>
/// Service for fetching exchange rates from the CurrencyFreaks API.
/// </summary>
public class CurrencyFreaksService : IFxRateProvider
{
    private const string BaseUrl = "https://api.currencyfreaks.com/v2.0/rates/latest";
    
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<CurrencyFreaksService> _logger;

    /// <summary>
    /// Creates a new instance of CurrencyFreaksService.
    /// </summary>
    /// <param name="httpClient">HttpClient instance configured for CurrencyFreaks API.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="logger">Logger instance.</param>
    public CurrencyFreaksService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CurrencyFreaksService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["CurrencyFreaks:ApiKey"] ?? throw new InvalidOperationException("CurrencyFreaks:ApiKey configuration is missing.");
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, decimal>> GetLatestRatesAsync(string baseCurrency = "USD", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching latest exchange rates from CurrencyFreaks for base currency: {BaseCurrency}", baseCurrency);
        
        try
        {
            var url = $"{BaseUrl}?apikey={_apiKey}&base={baseCurrency}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("CurrencyFreaks API request failed with status {StatusCode}: {ErrorContent}", 
                    response.StatusCode, errorContent);
                response.EnsureSuccessStatusCode();
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("CurrencyFreaks API response: {JsonContent}", jsonContent);
            
            var currencyFreaksResponse = JsonSerializer.Deserialize<CurrencyFreaksResponse>(jsonContent);

            if (currencyFreaksResponse?.Rates == null)
            {
                _logger.LogError("Failed to deserialize CurrencyFreaks response or rates are null. Response content: {JsonContent}", jsonContent);
                throw new InvalidOperationException("Invalid response from CurrencyFreaks API");
            }

            var rates = new Dictionary<string, decimal>();
            foreach (var rate in currencyFreaksResponse.Rates)
            {
                if (decimal.TryParse(rate.Value, out var rateValue))
                {
                    rates[rate.Key] = rateValue;
                }
            }

            _logger.LogInformation("Successfully retrieved {RateCount} exchange rates from CurrencyFreaks", rates.Count);
            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching exchange rates from CurrencyFreaks");
            throw;
        }
    }

    /// <summary>
    /// Internal response model for CurrencyFreaks API.
    /// </summary>
    private class CurrencyFreaksResponse
    {
        [JsonPropertyName("date")]
        public string? Date { get; set; }
        
        [JsonPropertyName("base")]
        public string? Base { get; set; }
        
        [JsonPropertyName("rates")]
        public Dictionary<string, string>? Rates { get; set; }
    }
}
