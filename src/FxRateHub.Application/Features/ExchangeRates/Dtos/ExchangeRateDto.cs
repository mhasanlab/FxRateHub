using System;

namespace FxRateHub.Application.Features.ExchangeRates.Dtos;

public record ExchangeRateDto(
    string BaseCurrency,
    string TargetCurrency,
    decimal Rate,
    DateTime UpdatedAt
);
