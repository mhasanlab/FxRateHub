using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Auth.DTOs;
using FxRateHub.Domain.Entities;
using FxRateHub.Domain.Enums;

namespace FxRateHub.Application.Features.Auth.Commands;

/// <summary>
/// Handler for the GoogleLoginCommand.
/// </summary>
public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJwtService _jwtService;

    public GoogleLoginCommandHandler(
        IApplicationDbContext dbContext,
        IGoogleAuthService googleAuthService,
        IJwtService jwtService)
    {
        _dbContext = dbContext;
        _googleAuthService = googleAuthService;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        GoogleLoginCommand request,
        CancellationToken cancellationToken)
    {
        // Validate Google token
        var googleUserInfo = await _googleAuthService.ValidateTokenAsync(request.IdToken);

        if (googleUserInfo == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid Google token");
        }

        // Find existing user by email (case-insensitive)
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == googleUserInfo.Email.ToLowerInvariant(), 
                cancellationToken);

        if (user != null)
        {
            // User exists, check auth provider
            if (user.AuthProvider != AuthProvider.Google)
            {
                return Result<AuthResponseDto>.Failure("This email is registered with a different login method");
            }

            // Check if user is blocked
            if (user.IsBlocked)
            {
                return Result<AuthResponseDto>.Failure("Your account has been blocked");
            }
        }
        else
        {
            // User doesn't exist, create new user
            user = Domain.Entities.User.CreateGoogleUser(
                googleUserInfo.Email,
                googleUserInfo.Name,
                googleUserInfo.GoogleId);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(user!);

        // Create response
        var response = new AuthResponseDto
        {
            UserId = user!.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
