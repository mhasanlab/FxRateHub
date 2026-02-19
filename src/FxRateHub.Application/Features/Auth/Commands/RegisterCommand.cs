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

namespace FxRateHub.Application.Features.Auth.Commands;

/// <summary>
/// Command for user registration.
/// </summary>
public record RegisterCommand : IRequest<Result<AuthResponseDto>>
{
    /// <summary>
    /// The email address for the new user account.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The password for the new user account.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// The full name of the new user.
    /// </summary>
    public string FullName { get; init; } = string.Empty;
}

/// <summary>
/// Validator for the RegisterCommand.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        // Email rules
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        // Password rules
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one number");

        // FullName rules
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
    }
}

/// <summary>
/// Handler for the RegisterCommand.
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        IApplicationDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Check if user with email already exists (case-insensitive)
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (existingUser != null)
        {
            return Result<AuthResponseDto>.Failure("Email already registered");
        }

        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Create new user
        var user = Domain.Entities.User.CreateLocalUser(request.Email, passwordHash, request.FullName);

        // Add user to database
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

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
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Result<AuthResponseDto>.Success(response);
    }
}
