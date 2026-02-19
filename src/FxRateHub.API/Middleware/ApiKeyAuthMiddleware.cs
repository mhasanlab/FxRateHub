using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Domain.Entities;

namespace FxRateHub.API.Middleware;

/// <summary>
/// Middleware that authenticates API requests using API keys passed via the X-API-Key header.
/// Validates the key, checks rate limits, tracks usage, and stores context items for downstream use.
/// </summary>
public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-API-Key";
    private const int DefaultDailyRequestLimit = 1000;

    public ApiKeyAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApplicationDbContext dbContext, IApiKeyService apiKeyService)
    {
        // Step 1: Check if path starts with "/api/v1"
        if (!context.Request.Path.StartsWithSegments("/api/v1"))
        {
            await _next(context);
            return;
        }

        // Step 2: Check for X-API-Key header
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey) ||
            string.IsNullOrWhiteSpace(extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 401,
                message = "API Key is missing"
            }));
            return;
        }

        var providedKey = extractedApiKey.ToString();

        // Step 3: Validate API key - hash using the same algorithm used in ApiKeyService (SHA256)
        var keyHash = apiKeyService.HashApiKey(providedKey);

        var apiKey = await dbContext.ApiKeys
            .Include(k => k.User)
            .FirstOrDefaultAsync(k => k.KeyHash == keyHash);

        if (apiKey == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 401,
                message = "Invalid API Key"
            }));
            return;
        }

        // Check if key is inactive or user is blocked
        if (!apiKey.IsActive || apiKey.User.IsBlocked)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 403,
                message = !apiKey.IsActive ? "API Key is inactive" : "User account is blocked"
            }));
            return;
        }

        // Step 4: Check rate limit
        var dailyLimitSetting = await dbContext.AppSettings
            .FirstOrDefaultAsync(s => s.SettingKey == "DailyRequestLimit");

        var dailyLimit = dailyLimitSetting != null && int.TryParse(dailyLimitSetting.SettingValue, out var parsedLimit)
            ? parsedLimit
            : DefaultDailyRequestLimit;

        if (apiKey.HasExceededDailyLimit(dailyLimit))
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.Date.AddDays(1);
            var resetTime = tomorrow - now;

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 429,
                message = "Daily API request limit exceeded",
                dailyLimit,
                currentUsage = apiKey.DailyRequestCount,
                resetsIn = $"{resetTime.Hours}h {resetTime.Minutes}m {resetTime.Seconds}s",
                resetsAt = tomorrow.ToString("O")
            }));
            return;
        }

        // Step 5: Track usage - increment counters
        apiKey.IncrementUsage();

        // Create ApiUsageLog entry with initial status code of 0 (will be updated after request)
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var usageLog = ApiUsageLog.Create(
            apiKeyId: apiKey.Id,
            endpoint: context.Request.Path,
            httpMethod: context.Request.Method,
            statusCode: 0,
            ipAddress: ipAddress
        );

        dbContext.ApiUsageLogs.Add(usageLog);
        await dbContext.SaveChangesAsync(context.RequestAborted);

        // Step 6: Store context items for downstream use
        context.Items["ApiKeyId"] = apiKey.Id;
        context.Items["UserId"] = apiKey.UserId;

        // Step 7: Call next middleware
        await _next(context);

        // Step 8: After next() completes, update the ApiUsageLog with actual response status code
        try
        {
            var logEntry = await dbContext.ApiUsageLogs.FindAsync(usageLog.Id);
            if (logEntry != null)
            {
                // Use ExecuteUpdateAsync to update the status code since ResponseStatusCode has a private setter
                await dbContext.ApiUsageLogs
                    .Where(l => l.Id == usageLog.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(l => l.ResponseStatusCode, context.Response.StatusCode));
            }
        }
        catch
        {
            // Silently handle any errors during status code update to not affect the response
        }
    }
}
