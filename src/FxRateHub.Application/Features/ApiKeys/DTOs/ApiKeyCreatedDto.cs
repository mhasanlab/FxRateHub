using System;

namespace FxRateHub.Application.Features.ApiKeys.DTOs;

/// <summary>
/// Data transfer object for newly created API key.
/// Contains the full API key which is only shown once.
/// </summary>
public record ApiKeyCreatedDto(
    Guid Id,
    string ApiKey,
    string KeyPrefix,
    DateTime CreatedAt,
    string Warning
);
