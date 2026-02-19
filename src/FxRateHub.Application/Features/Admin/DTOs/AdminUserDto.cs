using System;

namespace FxRateHub.Application.Features.Admin.DTOs;

public record AdminUserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string AuthProvider,
    bool IsBlocked,
    bool HasApiKey,
    int? DailyRequestCount,
    long? TotalRequestCount,
    DateTime CreatedAt,
    DateTime? LastRequestDate
);
