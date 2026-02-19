using System;

namespace FxRateHub.Application.Features.ApiKeys.DTOs;

/// <summary>
/// Data transfer object for API key details.
/// </summary>
public record ApiKeyDto(
    Guid Id,
    string KeyPrefix,
    int DailyRequestCount,
    long TotalRequestCount,
    int DailyLimit,
    DateTime? LastRequestDate,
    bool IsActive,
    DateTime CreatedAt
);
