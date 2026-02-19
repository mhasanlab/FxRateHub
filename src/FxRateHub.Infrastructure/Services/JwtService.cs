using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FxRateHub.Infrastructure.Services;

/// <summary>
/// Implements IJwtService for JWT token generation and validation.
/// </summary>
public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationDays;

    /// <summary>
    /// Initializes a new instance of the JwtService class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when SecretKey is missing from configuration.</exception>
    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException(nameof(configuration), "Jwt:SecretKey is required");
        _issuer = configuration["Jwt:Issuer"] ?? "FxRateHub";
        _audience = configuration["Jwt:Audience"] ?? "FxRateHubUsers";
        _expirationDays = configuration.GetValue<int>("Jwt:ExpirationDays", 7);
    }

    /// <inheritdoc />
    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("auth_provider", user.AuthProvider.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_expirationDays),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <inheritdoc />
    public (bool isValid, Guid userId) ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, Guid.Empty);
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return (false, Guid.Empty);
            }

            return (true, userId);
        }
        catch
        {
            return (false, Guid.Empty);
        }
    }
}
