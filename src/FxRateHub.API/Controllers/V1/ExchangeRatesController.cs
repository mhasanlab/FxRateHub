using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using FxRateHub.Application.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using FxRateHub.Application.Features.ExchangeRates.Queries;
using FxRateHub.Application.Common.Interfaces;

namespace FxRateHub.API.Controllers.V1;

/// <summary>
/// Controller for exchange rate operations (v1 API).
/// All endpoints require API key authentication via X-API-Key header.
/// </summary>
[ApiController]
[Route("api/v1")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "v1")]
[SwaggerTag("Exchange Rates")]
public class ExchangeRatesController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMediator _mediator;
    private const int DefaultDailyLimit = 1000;

    /// <summary>
    /// Initializes a new instance of the ExchangeRatesController.
    /// </summary>
    /// <param name="exchangeRateService">Service for exchange rate operations.</param>
    /// <param name="dbContext">Database context for tracking API usage.</param>
    /// <param name="mediator">Mediator for CQRS queries.</param>
    public ExchangeRatesController(
        IExchangeRateService exchangeRateService,
        IApplicationDbContext dbContext,
        IMediator mediator)
    {
        _exchangeRateService = exchangeRateService;
        _dbContext = dbContext;
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all exchange rates with an optional base currency.
    /// </summary>
    /// <param name="baseCurrency">The base currency code (e.g., USD). Defaults to USD if not specified.</param>
    /// <returns>Exchange rates for the specified base currency.</returns>
    /// <response code="200">Returns the exchange rates.</response>
    /// <response code="401">API key is missing or invalid.</response>
    /// <response code="429">Daily request limit exceeded.</response>
    [HttpGet("rates")]
    [SwaggerOperation(
        Summary = "Get all exchange rates",
        Description = "Retrieves all available exchange rates. Optionally specify a base currency to convert from.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the exchange rates", typeof(ExchangeRatesResponseDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "API key is missing or invalid")]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Daily request limit exceeded")]
    public async Task<IActionResult> GetExchangeRates([FromQuery] string? baseCurrency)
    {
        var result = await _exchangeRateService.GetExchangeRatesAsync(baseCurrency ?? "USD");
        SetRateLimitHeaders();
        return Ok(result);
    }

    /// <summary>
    /// Gets a specific exchange rate for a target currency.
    /// </summary>
    /// <param name="currencyCode">The target currency code (e.g., EUR).</param>
    /// <param name="baseCurrency">The base currency code. Defaults to USD.</param>
    /// <returns>The exchange rate for the specified currency pair.</returns>
    /// <response code="200">Returns the exchange rate.</response>
    /// <response code="401">API key is missing or invalid.</response>
    /// <response code="404">Exchange rate not found.</response>
    /// <response code="429">Daily request limit exceeded.</response>
    [HttpGet("rates/{currencyCode}")]
    [SwaggerOperation(
        Summary = "Get exchange rate for a specific currency",
        Description = "Retrieves the exchange rate for a specific currency pair.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the exchange rate", typeof(ExchangeRateDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "API key is missing or invalid")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Exchange rate not found")]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Daily request limit exceeded")]
    public async Task<IActionResult> GetExchangeRate(
        [FromRoute] string currencyCode,
        [FromQuery] string? baseCurrency)
    {
        var result = await _mediator.Send(new GetRateByCodeQuery(baseCurrency ?? "USD", currencyCode));
        SetRateLimitHeaders();
        return Ok(result);
    }

    /// <summary>
    /// Converts an amount from one currency to another.
    /// </summary>
    /// <param name="from">The source currency code (e.g., USD).</param>
    /// <param name="to">The target currency code (e.g., EUR).</param>
    /// <param name="amount">The amount to convert.</param>
    /// <returns>The conversion result with the converted amount and rate.</returns>
    /// <response code="200">Returns the conversion result.</response>
    /// <response code="400">Invalid currency codes or amount.</response>
    /// <response code="401">API key is missing or invalid.</response>
    /// <response code="429">Daily request limit exceeded.</response>
    [HttpGet("convert")]
    [SwaggerOperation(
        Summary = "Convert currency",
        Description = "Converts a specified amount from one currency to another.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the conversion result", typeof(ConversionResultDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid currency codes or amount")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "API key is missing or invalid")]
    [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Daily request limit exceeded")]
    public async Task<IActionResult> ConvertCurrency(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] decimal amount)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return BadRequest("From and To currency codes are required.");
        }

        if (amount <= 0)
        {
            return BadRequest("Amount must be greater than zero.");
        }

        var result = await _exchangeRateService.ConvertCurrencyAsync(from, to, amount);
        SetRateLimitHeaders();
        return Ok(result);
    }

    /// <summary>
    /// Sets rate limit headers on the response.
    /// Headers include:
    /// - X-RateLimit-Limit: The daily request limit.
    /// - X-RateLimit-Remaining: The remaining requests for the day.
    /// - X-RateLimit-Reset: The UTC timestamp when the rate limit resets (midnight).
    /// </summary>
    private void SetRateLimitHeaders()
    {
        var now = DateTime.UtcNow;
        var tomorrow = now.Date.AddDays(1);
        var remaining = DefaultDailyLimit;

        // Get the current API key usage from context if available
        if (HttpContext.Items.TryGetValue("ApiKeyId", out var apiKeyId) && apiKeyId != null)
        {
            // In a real implementation, you would fetch the current usage from the database
            // For now, we'll use the default limit
        }

        Response.Headers.Append("X-RateLimit-Limit", DefaultDailyLimit.ToString());
        Response.Headers.Append("X-RateLimit-Remaining", remaining.ToString());
        Response.Headers.Append("X-RateLimit-Reset", ((DateTimeOffset)tomorrow).ToUnixTimeSeconds().ToString());
    }
}
