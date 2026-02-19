using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ApiKeys.DTOs;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Features.ApiKeys.Queries;

/// <summary>
/// Query to retrieve the current user's API key details.
/// </summary>
public record GetApiKeyQuery : IRequest<ApiKeyDto?>
{
}

/// <summary>
/// Handler for the GetApiKeyQuery.
/// </summary>
public class GetApiKeyQueryHandler : IRequestHandler<GetApiKeyQuery, ApiKeyDto?>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetApiKeyQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ApiKeyDto?> Handle(
        GetApiKeyQuery request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            throw new ForbiddenException("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;

        // Get daily limit from AppSettings
        var dailyLimitSetting = await _dbContext.AppSettings
            .FirstOrDefaultAsync(s => s.SettingKey == "ApiDailyLimit", cancellationToken);

        int dailyLimit = dailyLimitSetting != null && int.TryParse(dailyLimitSetting.SettingValue, out var limit)
            ? limit
            : 1000;

        // Get API key for current user
        var apiKey = await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.UserId == userId, cancellationToken);

        if (apiKey == null)
        {
            return null;
        }

        // Calculate today's request count from ApiUsageLogs
        var today = DateTime.Today;
        var todayRequestCount = await _dbContext.ApiUsageLogs
            .CountAsync(log => log.ApiKeyId == apiKey.Id && log.CreatedAt.Date == today, cancellationToken);

        // Get total usage count
        var totalRequestCount = await _dbContext.ApiUsageLogs
            .CountAsync(log => log.ApiKeyId == apiKey.Id, cancellationToken);

        // Return DTO with masked key prefix (first 8 characters + "...")
        var maskedKeyPrefix = apiKey.KeyPrefix.Length > 8
            ? apiKey.KeyPrefix.Substring(0, 8) + "..."
            : apiKey.KeyPrefix;

        return new ApiKeyDto(
            apiKey.Id,
            maskedKeyPrefix,
            todayRequestCount,
            totalRequestCount,
            dailyLimit,
            apiKey.LastRequestDate,
            apiKey.IsActive,
            apiKey.CreatedAt
        );
    }
}
