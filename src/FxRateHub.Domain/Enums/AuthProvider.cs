namespace FxRateHub.Domain.Enums;

/// <summary>
/// Represents the authentication provider used for user authentication.
/// </summary>
public enum AuthProvider
{
    /// <summary>
    /// Local authentication using username and password.
    /// </summary>
    Local = 0,
    
    /// <summary>
    /// Authentication via Google OAuth.
    /// </summary>
    Google = 1
}
