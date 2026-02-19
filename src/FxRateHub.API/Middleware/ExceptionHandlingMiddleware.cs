using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FxRateHub.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace FxRateHub.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ValidationException => 400,
            NotFoundException => 404,
            ForbiddenException => 403,
            RateLimitExceededException => 429,
            _ => 500
        };

        response.StatusCode = statusCode;

        var errorResponse = new ErrorResponse
        {
            Status = statusCode,
            Message = exception.Message
        };

        if (exception is ValidationException validationException)
        {
            errorResponse.Errors = validationException.Errors;
        }

        if (statusCode == 500)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }
}

public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public IDictionary<string, string[]>? Errors { get; set; }
}
