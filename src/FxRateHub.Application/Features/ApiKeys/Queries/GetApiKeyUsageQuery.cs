using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ApiKeys.DTOs;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Features.ApiKeys.Queries;

/// <summary>
/// Query to retrieve the current user's API key usage statistics.
/// </summary>
public record GetApiKeyUsageQuery : IRequest<ApiKeyUsageDto?>
{
}

/// <summary>
/// Handler for the GetApiKeyUsageQuery.
/// </summary>
public class GetApiKeyUsageQueryHandler : IRequestHandler<GetApiKeyUsageQuery, ApiKeyUsageDto?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetApiKeyUsageQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ApiKeyUsageDto?> Handle(
        GetApiKeyUsageQuery request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            return null;
        }

        var userId = _currentUserService.UserId.Value;

        // Get API key for current user with UsageLogs included
        var apiKey = await _dbContext.ApiKeys
            .Include(k => k.UsageLogs)
            .FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);

        if (apiKey == null)
        {
            return null;
        }

        // Get daily limit from AppSettings
        var dailyLimitSetting = await _dbContext.AppSettings
            .FirstOrDefaultAsync(s => s.SettingKey == "ApiDailyLimit", cancellationToken);

        int dailyLimit = dailyLimitSetting != null && int.TryParse(dailyLimitSetting.SettingValue, out var limit)
            ? limit
            : 1000;

        var today = DateTime.Today;

        // Check if last reset date is different from today - if so, reset daily usage count
        if (apiKey.LastRequestDate?.Date != today)
        {
            apiKey.ResetDailyCount();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Get today's request count from UsageLogs
        var todayRequestCount = apiKey.UsageLogs
            .Count(log => log.CreatedAt.Date == today);

        // Calculate total requests from UsageLogs
        var totalRequestCount = apiKey.UsageLogs.Count;

        // Get last 7 days usage history
        var sevenDaysAgo = today.AddDays(-6);
        var last7DaysLogs = apiKey.UsageLogs
            .Where(log => log.CreatedAt.Date >= sevenDaysAgo && log.CreatedAt.Date <= today)
            .ToList();

        // Group by date and count
        var dailyUsageHistory = last7DaysLogs
            .GroupBy(log => log.CreatedAt.Date)
            .Select(g => new DailyUsageDto(g.Key, g.Count()))
            .OrderBy(d => d.Date)
            .ToList();

        // Ensure we have entries for all 7 days (including days with 0 usage)
        var allDays = new List<DailyUsageDto>();
        for (var i = 0; i < 7; i++)
        {
            var date = today.AddDays(-6 + i);
            var existingEntry = dailyUsageHistory.FirstOrDefault(d => d.Date == date);
            allDays.Add(existingEntry ?? new DailyUsageDto(date, 0));
        }

        return new ApiKeyUsageDto(
            todayRequestCount,
            dailyLimit,
            dailyLimit - todayRequestCount,
            totalRequestCount,
            allDays
        );
    }
}
