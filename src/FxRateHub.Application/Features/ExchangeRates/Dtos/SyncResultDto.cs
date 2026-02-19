using System;
using System.Collections.Generic;

namespace FxRateHub.Application.Features.ExchangeRates.Dtos;

public record SyncResultDto(
    int CurrenciesUpdated,
    DateTime LastSyncTime,
    bool Success,
    string? ErrorMessage = null
);
