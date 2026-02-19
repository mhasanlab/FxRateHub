using System;

namespace FxRateHub.Application.Features.Auth.DTOs;

/// <summary>
/// User data transfer object containing user information.
/// </summary>
public record UserDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// The role of the user (e.g., User, Admin).
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// The authentication provider used (e.g., Local, Google).
    /// </summary>
    public string AuthProvider { get; init; } = string.Empty;

    /// <summary>
    /// Indicates whether the user account is blocked.
    /// </summary>
    public bool IsBlocked { get; init; }

    /// <summary>
    /// The date and time when the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
