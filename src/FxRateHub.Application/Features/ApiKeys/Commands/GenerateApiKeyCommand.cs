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
/// Command to generate a new API key for the current user.
/// </summary>
public record GenerateApiKeyCommand : IRequest<Result<ApiKeyCreatedDto>>
{
}

/// <summary>
/// Handler for the GenerateApiKeyCommand.
/// </summary>
public class GenerateApiKeyCommandHandler : IRequestHandler<GenerateApiKeyCommand, Result<ApiKeyCreatedDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApiKeyService _apiKeyService;

    public GenerateApiKeyCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IApiKeyService apiKeyService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _apiKeyService = apiKeyService;
    }

    public async Task<Result<ApiKeyCreatedDto>> Handle(
        GenerateApiKeyCommand request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            throw new ForbiddenException("You must be logged in to generate an API key.");
        }

        var userId = _currentUserService.UserId.Value;

        // Check if user already has an active API key
        var existingApiKey = await _dbContext.ApiKeys
            .FirstOrDefaultAsync(k => k.UserId == userId && k.IsActive, cancellationToken);

        if (existingApiKey != null)
        {
            return Result<ApiKeyCreatedDto>.Failure("You already have an active API key. Please regenerate it if you need a new one.");
        }

        // Generate new API key
        var apiKey = _apiKeyService.GenerateApiKey();
        var keyHash = _apiKeyService.HashApiKey(apiKey);
        var keyPrefix = _apiKeyService.GetKeyPrefix(apiKey);

        // Create API key entity
        var newApiKey = ApiKey.Create(userId, keyHash, keyPrefix);

        _dbContext.ApiKeys.Add(newApiKey);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Return the created API key
        var result = new ApiKeyCreatedDto(
            newApiKey.Id,
            apiKey,
            keyPrefix,
            newApiKey.CreatedAt,
            "Make sure to save this API key. You won't be able to see it again."
        );

        return Result<ApiKeyCreatedDto>.Success(result);
    }
}
