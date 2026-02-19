using System;

namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for the current user service.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is an administrator.
    /// </summary>
    bool IsAdmin { get; }
}
