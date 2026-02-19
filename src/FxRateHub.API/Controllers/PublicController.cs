using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FxRateHub.Application.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;

namespace FxRateHub.API.Controllers;

/// <summary>
/// Public API controller for exchange rate operations (no authentication required)
/// </summary>
[ApiController]
[Route("api/public")]
[Produces("application/json")]
public class PublicController : ControllerBase
{
    private readonly IExchangeRateService _exchangeRateService;

    public PublicController(IExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    /// <summary>
    /// Get all latest exchange rates
    /// </summary>
    /// <param name="baseCurrency">Base currency code (default: USD)</param>
    /// <returns>List of exchange rates with the specified base currency</returns>
    /// <response code="200">Returns the exchange rates</response>
    [HttpGet("rates")]
    [ProducesResponseType(typeof(ExchangeRatesResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRates([FromQuery] string baseCurrency = "USD")
    {
        var rates = await _exchangeRateService.GetExchangeRatesAsync(baseCurrency);
        
        Response.HttpContext.Response.Headers["Cache-Control"] = "public,max-age=300";
        
        return Ok(rates);
    }

    /// <summary>
    /// Convert an amount from one currency to another
    /// </summary>
    /// <param name="from">Source currency code (e.g., USD)</param>
    /// <param name="to">Target currency code (e.g., EUR)</param>
    /// <param name="amount">Amount to convert</param>
    /// <returns>Conversion result with converted amount and rate</returns>
    /// <response code="200">Returns the conversion result</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Currency not found</response>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(ConversionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Convert(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] decimal amount)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return BadRequest("From and To currencies are required.");
        }

        if (amount <= 0)
        {
            return BadRequest("Amount must be greater than zero.");
        }

        try
        {
            var result = await _exchangeRateService.ConvertCurrencyAsync(from, to, amount);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Currency '{from}' or '{to}' not found.");
        }
    }

    /// <summary>
    /// Get list of available currency codes
    /// </summary>
    /// <returns>List of supported currency codes</returns>
    /// <response code="200">Returns the list of currencies</response>
    [HttpGet("currencies")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrencies()
    {
        var currencies = await _exchangeRateService.GetAvailableCurrenciesAsync();
        return Ok(currencies);
    }
}
