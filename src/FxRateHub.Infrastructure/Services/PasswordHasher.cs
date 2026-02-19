using FxRateHub.Application.Common.Interfaces;

namespace FxRateHub.Infrastructure.Services;

/// <summary>
/// Implements IPasswordHasher for password hashing and verification using BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    /// <inheritdoc />
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
    }

    /// <inheritdoc />
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
