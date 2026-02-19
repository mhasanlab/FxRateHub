using System;

namespace FxRateHub.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for API key generation and verification services.
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// Generates a new API key.
    /// </summary>
    /// <returns>A newly generated API key string.</returns>
    string GenerateApiKey();

    /// <summary>
    /// Hashes an API key for secure storage.
    /// </summary>
    /// <param name="apiKey">The API key to hash.</param>
    /// <returns>A hashed version of the API key.</returns>
    string HashApiKey(string apiKey);

    /// <summary>
    /// Verifies an API key against a stored hash.
    /// </summary>
    /// <param name="apiKey">The API key to verify.</param>
    /// <param name="hash">The stored hash to compare against.</param>
    /// <returns>True if the API key matches the hash; otherwise, false.</returns>
    bool VerifyApiKey(string apiKey, string hash);

    /// <summary>
    /// Gets the prefix of an API key for identification purposes.
    /// </summary>
    /// <param name="apiKey">The API key to get the prefix from.</param>
    /// <returns>The first few characters of the API key for identification.</returns>
    string GetKeyPrefix(string apiKey);
}
