namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for password hashing and verification services.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password for secure storage.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>A hashed version of the password.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifies a password against a stored hash.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hash">The stored hash to compare against.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    bool Verify(string password, string hash);
}
