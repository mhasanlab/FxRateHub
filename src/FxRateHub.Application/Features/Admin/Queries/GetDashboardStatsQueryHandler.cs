using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;
    
    public GetDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        
        var totalUsers = await _context.Users.CountAsync(cancellationToken);
        var activeUsers = await _context.Users.CountAsync(u => !u.IsBlocked, cancellationToken);
        var blockedUsers = await _context.Users.CountAsync(u => u.IsBlocked, cancellationToken);
        var totalApiKeys = await _context.ApiKeys.CountAsync(cancellationToken);
        var totalApiRequests = await _context.ApiUsageLogs.CountAsync(cancellationToken);
        var todayApiRequests = await _context.ApiUsageLogs.CountAsync(l => l.RequestedAt.Date == today, cancellationToken);
        var totalCurrencies = await _context.ExchangeRates.CountAsync(cancellationToken);
        
        var lastSync = await _context.SyncLogs
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
        
        var recentSyncs = await _context.SyncLogs
            .OrderByDescending(s => s.StartedAt)
            .Take(5)
            .Select(s => new RecentSyncDto(
                s.Id,
                s.Status.ToString(),
                s.CurrenciesUpdated,
                s.StartedAt,
                s.CompletedAt
            ))
            .ToListAsync(cancellationToken);
        
        return new DashboardStatsDto(
            totalUsers,
            activeUsers,
            blockedUsers,
            totalApiKeys,
            totalApiRequests,
            todayApiRequests,
            totalCurrencies,
            lastSync?.StartedAt,
            lastSync?.Status.ToString(),
            recentSyncs
        );
    }
}
