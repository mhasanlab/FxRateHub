using System;

namespace FxRateHub.Application.Features.ApiKeys.DTOs;

/// <summary>
/// Data transfer object for daily usage statistics.
/// </summary>
public record DailyUsageDto(
    DateTime Date,
    int RequestCount
);
