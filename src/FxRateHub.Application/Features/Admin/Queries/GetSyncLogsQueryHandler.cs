using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Queries;

public class GetSyncLogsQueryHandler : IRequestHandler<GetSyncLogsQuery, List<RecentSyncDto>>
{
    private readonly IApplicationDbContext _context;
    
    public GetSyncLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<RecentSyncDto>> Handle(GetSyncLogsQuery request, CancellationToken cancellationToken)
    {
        return await _context.SyncLogs
            .OrderByDescending(s => s.StartedAt)
            .Take(request.Count)
            .Select(s => new RecentSyncDto(
                s.Id,
                s.Status.ToString(),
                s.CurrenciesUpdated,
                s.StartedAt,
                s.CompletedAt
            ))
            .ToListAsync(cancellationToken);
    }
}
