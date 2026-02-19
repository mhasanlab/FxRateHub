using System;

namespace FxRateHub.Application.Features.Auth.DTOs;

/// <summary>
/// Request DTO for Google OAuth login.
/// </summary>
public record GoogleLoginRequest
{
    /// <summary>
    /// The Google ID token received from the Google OAuth flow.
    /// </summary>
    public string IdToken { get; init; } = string.Empty;
}
