using System;
using System.Collections.Generic;

namespace FxRateHub.Application.Features.ExchangeRates.Dtos;

/// <summary>
/// Response DTO for currency conversion
/// </summary>
public record ConversionResultDto(
    string From,
    string To,
    decimal Amount,
    decimal ConvertedAmount,
    decimal Rate,
    DateTime Timestamp
);
