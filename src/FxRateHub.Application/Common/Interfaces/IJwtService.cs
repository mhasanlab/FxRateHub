using System;
using FxRateHub.Domain.Entities;

namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for JWT token generation and validation services.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate a token for.</param>
    /// <returns>A JWT token string.</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Validates a JWT token and returns the validation result.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>A tuple containing the validation status and user ID if valid.</returns>
    (bool isValid, Guid userId) ValidateToken(string token);
}
