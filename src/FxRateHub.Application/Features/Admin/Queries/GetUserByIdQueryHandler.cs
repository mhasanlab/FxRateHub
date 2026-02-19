using System;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Admin.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<AdminUserDto>>
{
    private readonly IApplicationDbContext _context;
    
    public GetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<AdminUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.ApiKey)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        
        if (user == null)
            throw new NotFoundException("User", request.UserId);
        
        var adminUserDto = new AdminUserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.Role.ToString(),
            user.AuthProvider.ToString(),
            user.IsBlocked,
            user.ApiKey != null,
            null,
            null,
            user.CreatedAt,
            user.ApiKey?.LastRequestDate
        );
        
        return Result<AdminUserDto>.Success(adminUserDto);
    }
}
