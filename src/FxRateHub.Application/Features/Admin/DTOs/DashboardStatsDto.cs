using System;
using System.Collections.Generic;

namespace FxRateHub.Application.Features.Admin.DTOs;

public record DashboardStatsDto(
    int TotalUsers,
    int ActiveUsers,
    int BlockedUsers,
    int TotalApiKeys,
    long TotalApiRequests,
    int TodayApiRequests,
    int TotalCurrencies,
    DateTime? LastSyncTime,
    string? LastSyncStatus,
    List<RecentSyncDto> RecentSyncs
);
