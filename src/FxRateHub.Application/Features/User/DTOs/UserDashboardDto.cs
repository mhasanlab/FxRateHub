using System;
using System.Collections.Generic;
using FxRateHub.Application.Features.ApiKeys.DTOs;
using FxRateHub.Application.Features.ExchangeRates.Dtos;

namespace FxRateHub.Application.Features.User.DTOs;

/// <summary>
/// Data transfer object for the user dashboard containing profile, API key, usage, and top currencies.
/// </summary>
public record UserDashboardDto(
    UserDto Profile,
    ApiKeyDto? ApiKey,
    ApiKeyUsageDto? Usage,
    List<ExchangeRateDto> TopCurrencies
);
