using System;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using FxRateHub.Application.Common.Interfaces;

namespace FxRateHub.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
    {
        _clientId = configuration["Google:ClientId"] 
            ?? throw new InvalidOperationException("Google:ClientId is not configured");
        _logger = logger;
    }

    public async Task<GoogleUserInfo?> ValidateTokenAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _clientId }
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            
            var name = string.IsNullOrEmpty(payload.Name) 
                ? payload.Email.Split('@')[0] 
                : payload.Name;

            return new GoogleUserInfo(
                Email: payload.Email,
                Name: name,
                GoogleId: payload.Subject
            );
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google ID token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Google ID token");
            return null;
        }
    }
}
