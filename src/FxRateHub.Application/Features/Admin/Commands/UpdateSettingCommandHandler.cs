using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Commands;

public class UpdateSettingCommandHandler : IRequestHandler<UpdateSettingCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateSettingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<string>> Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        var setting = await _context.AppSettings.FirstOrDefaultAsync(s => s.SettingKey == request.Key);
        
        if (setting == null)
            throw new NotFoundException("AppSetting", request.Key);
        
        setting.UpdateValue(request.Value);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<string>.Success("Setting updated successfully.");
    }
}
