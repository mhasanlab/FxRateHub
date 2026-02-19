using System;
using System.Collections.Generic;

namespace FxRateHub.Application.Features.ApiKeys.DTOs;

/// <summary>
/// Data transfer object for API key usage statistics.
/// </summary>
public record ApiKeyUsageDto(
    int TodayRequests,
    int DailyLimit,
    int RemainingToday,
    long TotalRequests,
    List<DailyUsageDto> Last7DaysUsage
);
