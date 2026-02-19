using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.User.DTOs;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Features.User.Commands;

/// <summary>
/// Command to update the current user's profile.
/// </summary>
public record UpdateProfileCommand : IRequest<Result<UserDto>>
{
    /// <summary>
    /// The new full name for the user.
    /// </summary>
    public string FullName { get; init; } = string.Empty;
}

/// <summary>
/// Validator for the UpdateProfileCommand.
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);
    }
}

/// <summary>
/// Handler for the UpdateProfileCommand.
/// </summary>
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProfileCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserDto>> Handle(
        UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            return Result<UserDto>.Failure("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;

        // Get user from database
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), userId);
        }

        // Update user's full name
        user.FullName = request.FullName;

        // Save changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return updated user DTO
        return Result<UserDto>.Success(new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.CreatedAt
        ));
    }
}
