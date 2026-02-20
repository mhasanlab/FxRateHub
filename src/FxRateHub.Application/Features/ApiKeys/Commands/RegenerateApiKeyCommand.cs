using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.ApiKeys.DTOs;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Features.ApiKeys.Commands;

/// <summary>
/// Command to regenerate (replace) the current user's API key.
/// This will invalidate the existing API key and create a new one.
/// </summary>
public record RegenerateApiKeyCommand : IRequest<Result<ApiKeyCreatedDto>>
{
}

/// <summary>
/// Handler for the RegenerateApiKeyCommand.
/// </summary>
public class RegenerateApiKeyCommandHandler : IRequestHandler<RegenerateApiKeyCommand, Result<ApiKeyCreatedDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApiKeyService _apiKeyService;

    public RegenerateApiKeyCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IApiKeyService apiKeyService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _apiKeyService = apiKeyService;
    }

    public async Task<Result<ApiKeyCreatedDto>> Handle(
        RegenerateApiKeyCommand request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            throw new ForbiddenException("You must be logged in to regenerate an API key.");
        }

        var userId = _currentUserService.UserId.Value;

        // Find existing active API key
        var existingApiKey = await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.UserId == userId && k.IsActive, cancellationToken);

        if (existingApiKey == null)
        {
            return Result<ApiKeyCreatedDto>.Failure("No active API key found. Please generate a new one.");
        }

        // Generate new API key
        var newApiKey = _apiKeyService.GenerateApiKey();
        var keyHash = _apiKeyService.HashApiKey(newApiKey);
        var keyPrefix = _apiKeyService.GetKeyPrefix(newApiKey);

        // Update the existing API key with new values (instead of creating a new record)
        // Note: We preserve DailyRequestCount and TotalRequestCount to prevent rate limit bypass
        existingApiKey.KeyHash = keyHash;
        existingApiKey.KeyPrefix = keyPrefix;
        existingApiKey.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return the new API key
        var result = new ApiKeyCreatedDto(
            existingApiKey.Id,
            newApiKey,
            keyPrefix,
            existingApiKey.CreatedAt,
            "Make sure to save this new API key. You won't be able to see it again. Your old API key has been invalidated."
        );

        return Result<ApiKeyCreatedDto>.Success(result);
    }
}
