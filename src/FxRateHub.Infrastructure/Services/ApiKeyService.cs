using System;
using System.Security.Cryptography;
using FxRateHub.Application.Common.Interfaces;

namespace FxRateHub.Infrastructure.Services;

/// <summary>
/// Implements IApiKeyService for API key generation and verification.
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private const string Prefix = "fxr_";
    private const int KeyLength = 32;

    /// <inheritdoc />
    public string GenerateApiKey()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(KeyLength);
        var base64 = Convert.ToBase64String(randomBytes);
        
        // Remove +, /, = characters for URL-safe key
        var sanitized = base64.Replace('+', 'a')
                              .Replace('/', 'b')
                              .Replace('=', 'c');
        
        // Take first 32 characters and prepend prefix
        var keyPart = sanitized[..KeyLength];
        return $"{Prefix}{keyPart}";
    }

    /// <inheritdoc />
    public string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(apiKey);
        var hashBytes = sha256.ComputeHash(bytes);
        
        // Return uppercase hex string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
    }

    /// <inheritdoc />
    public bool VerifyApiKey(string apiKey, string hash)
    {
        var computedHash = HashApiKey(apiKey);
        return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public string GetKeyPrefix(string apiKey)
    {
        if (apiKey.Length <= 12)
        {
            return apiKey;
        }
        return apiKey[..12] + "...";
    }
}
