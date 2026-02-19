using System;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Domain.Entities;
using FxRateHub.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.Admin.Commands;

public class BlockUserCommandHandler : IRequestHandler<BlockUserCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    
    public BlockUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Result<string>> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(request.UserId);
        
        if (user == null)
            throw new NotFoundException("User", request.UserId);
        
        if (user.Role == UserRole.Admin && request.Block)
            return Result<string>.Failure("Cannot block an admin user.");
        
        user.IsBlocked = request.Block;
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<string>.Success(request.Block ? "User blocked successfully." : "User unblocked successfully.");
    }
}
