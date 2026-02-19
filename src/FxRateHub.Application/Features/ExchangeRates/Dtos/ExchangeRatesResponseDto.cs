using System;
using System.Collections.Generic;

namespace FxRateHub.Application.Features.ExchangeRates.Dtos;

/// <summary>
/// Response DTO for exchange rates
/// </summary>
public record ExchangeRatesResponseDto(
    string BaseCurrency,
    DateTime Timestamp,
    Dictionary<string, decimal> Rates
);
