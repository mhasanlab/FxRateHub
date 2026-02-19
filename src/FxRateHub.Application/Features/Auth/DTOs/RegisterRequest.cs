using System;

namespace FxRateHub.Application.Features.Auth.DTOs;

/// <summary>
/// Request DTO for user registration.
/// </summary>
public record RegisterRequest
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
