using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FxRateHub.Domain.Common;
using FxRateHub.Domain.Enums;

namespace FxRateHub.Domain.Entities;

/// <summary>
/// Represents a user entity in the system.
/// </summary>
public class User : BaseEntity<Guid>
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Column(TypeName = "varchar(255)")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The hashed password for local authentication. Nullable for OAuth users.
    /// </summary>
    [MaxLength(255)]
    public string? PasswordHash { get; set; }

    /// <summary>
    /// The user's full name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// The user's role in the system.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;

    /// <summary>
    /// The authentication provider used for login.
    /// </summary>
    public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;

    /// <summary>
    /// The Google ID for Google OAuth users. Nullable for local users.
    /// </summary>
    [MaxLength(128)]
    public string? GoogleId { get; set; }

    /// <summary>
    /// Indicates whether the user is blocked from the system.
    /// </summary>
    public bool IsBlocked { get; set; } = false;

    /// <summary>
    /// Navigation property to the associated API key.
    /// </summary>
    public ApiKey? ApiKey { get; set; }

    /// <summary>
    /// Creates a new local user with the specified email, password hash, and full name.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="passwordHash">The hashed password.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <returns>A new User instance configured as a local user.</returns>
    public static User CreateLocalUser(string email, string passwordHash, string fullName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = UserRole.User,
            AuthProvider = AuthProvider.Local,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a new Google OAuth user with the specified email, full name, and Google ID.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <param name="googleId">The Google ID from OAuth.</param>
    /// <returns>A new User instance configured as a Google OAuth user.</returns>
    public static User CreateGoogleUser(string email, string fullName, string googleId)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            FullName = fullName,
            GoogleId = googleId,
            Role = UserRole.User,
            AuthProvider = AuthProvider.Google,
            PasswordHash = null,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a new admin user with the specified email, password hash, and full name.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="passwordHash">The hashed password.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <returns>A new User instance configured as an admin user.</returns>
    public static User CreateAdmin(string email, string passwordHash, string fullName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName,
            Role = UserRole.Admin,
            AuthProvider = AuthProvider.Local,
            CreatedAt = DateTime.UtcNow
        };
    }
}
