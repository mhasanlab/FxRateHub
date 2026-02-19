using System;

namespace FxRateHub.Application.Features.Auth.DTOs;

/// <summary>
/// Request DTO for user login.
/// </summary>
public record LoginRequest
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
