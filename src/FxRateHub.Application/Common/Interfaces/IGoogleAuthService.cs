using System.Threading.Tasks;

namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Represents user information returned from Google authentication.
/// </summary>
public record GoogleUserInfo(string Email, string Name, string GoogleId);

/// <summary>
/// Defines the contract for Google authentication services.
/// </summary>
public interface IGoogleAuthService
{
    /// <summary>
    /// Validates a Google ID token and returns user information.
    /// </summary>
    /// <param name="idToken">The Google ID token to validate.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user information if valid; otherwise, null.</returns>
    Task<GoogleUserInfo?> ValidateTokenAsync(string idToken);
}
