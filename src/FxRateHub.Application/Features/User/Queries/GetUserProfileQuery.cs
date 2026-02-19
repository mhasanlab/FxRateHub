using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.User.DTOs;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Features.User.Queries;

/// <summary>
/// Query to retrieve the current user's profile.
/// </summary>
public record GetUserProfileQuery : IRequest<UserDto>
{
}

/// <summary>
/// Handler for the GetUserProfileQuery.
/// </summary>
public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserProfileQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            throw new ForbiddenException("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;

        // Get user from database
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        return new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.CreatedAt
        );
    }
}
