using System;

namespace FxRateHub.Application.Features.User.DTOs;

/// <summary>
/// Data transfer object for user profile information.
/// </summary>
public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    DateTime CreatedAt
);
