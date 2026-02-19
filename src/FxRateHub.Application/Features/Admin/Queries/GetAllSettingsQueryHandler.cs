using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Queries;

public class GetAllSettingsQueryHandler : IRequestHandler<GetAllSettingsQuery, List<AppSettingDto>>
{
    private readonly IApplicationDbContext _context;
    
    public GetAllSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<AppSettingDto>> Handle(GetAllSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _context.AppSettings
            .OrderBy(s => s.SettingKey)
            .Select(s => new AppSettingDto(
                s.Id,
                s.SettingKey,
                s.SettingValue,
                s.Description
            ))
            .ToListAsync(cancellationToken);
        
        return settings;
    }
}
