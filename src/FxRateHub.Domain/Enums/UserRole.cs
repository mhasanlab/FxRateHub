namespace FxRateHub.Domain.Enums;

/// <summary>
/// Represents the role of a user in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Standard user with basic permissions.
    /// </summary>
    User = 0,
    
    /// <summary>
    /// Administrator with elevated permissions.
    /// </summary>
    Admin = 1
}
