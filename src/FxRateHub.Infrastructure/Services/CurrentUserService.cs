using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FxRateHub.Application.Common.Interfaces;

namespace FxRateHub.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claim))
                return null;

            return Guid.TryParse(claim, out var userId) ? userId : null;
        }
    }

    public string? Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }

    public bool IsAdmin
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }
    }
}
