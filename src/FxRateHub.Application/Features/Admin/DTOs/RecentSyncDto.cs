using System;

namespace FxRateHub.Application.Features.Admin.DTOs;

public record RecentSyncDto(
    int Id,
    string Status,
    int CurrenciesUpdated,
    DateTime StartedAt,
    DateTime? CompletedAt
);
