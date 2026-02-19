using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Auth.DTOs;
using FxRateHub.Domain.Entities;
using FxRateHub.Domain.Enums;

namespace FxRateHub.Application.Features.Auth.Commands;

/// <summary>
/// Command for user login.
/// </summary>
public record LoginCommand : IRequest<Result<AuthResponseDto>>
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The password of the user.
    /// </summary>
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Validator for the LoginCommand.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IApplicationDbContext dbContext)
    {
        // Email rules
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        // Password rules
        RuleFor(x => x.Password)
            .NotEmpty();
    }
}

/// <summary>
/// Handler for the LoginCommand.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // Find user by email (case-insensitive) where AuthProvider is Local
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant() 
                                   && u.AuthProvider == AuthProvider.Local, cancellationToken);

        if (user == null)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        // Check if user is blocked
        if (user.IsBlocked)
        {
            return Result<AuthResponseDto>.Failure("Your account has been blocked");
        }

        // Verify password
        if (!string.IsNullOrEmpty(user.PasswordHash) && 
            !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password");
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(user);

        // Create response
        var response = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
