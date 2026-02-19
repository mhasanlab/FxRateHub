using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using FxRateHub.Application.Features.ApiKeys.DTOs;
using FxRateHub.Application.Features.ApiKeys.Commands;
using FxRateHub.Application.Features.ApiKeys.Queries;

namespace FxRateHub.API.Controllers;

[ApiController]
[Route("api/apikey")]
[Authorize]
[Produces("application/json")]
[SwaggerTag("ApiKey")]
public class ApiKeyController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiKeyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current user's API key info
    /// </summary>
    /// <returns>The API key information for the current user</returns>
    [HttpGet]
    [SwaggerOperation(Summary = "Get current user's API key info")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the API key information", typeof(ApiKeyDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No API key exists for the user")]
    public async Task<IActionResult> GetApiKey()
    {
        try
        {
            var result = await _mediator.Send(new GetApiKeyQuery());
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("No API key exists for this user.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Generate new API key for current user
    /// </summary>
    /// <returns>The newly generated API key (shown only once)</returns>
    [HttpPost("generate")]
    [SwaggerOperation(Summary = "Generate new API key for current user")]
    [SwaggerResponse(StatusCodes.Status201Created, "API key generated successfully", typeof(ApiKeyCreatedDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "User already has an API key")]
    public async Task<IActionResult> GenerateApiKey()
    {
        try
        {
            var result = await _mediator.Send(new GenerateApiKeyCommand());
            return CreatedAtAction(nameof(GetApiKey), result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Regenerate API key (invalidates old one)
    /// </summary>
    /// <returns>The newly generated API key (shown only once)</returns>
    [HttpPost("regenerate")]
    [SwaggerOperation(Summary = "Regenerate API key (invalidates old one)")]
    [SwaggerResponse(StatusCodes.Status201Created, "API key regenerated successfully", typeof(ApiKeyCreatedDto))]
    public async Task<IActionResult> RegenerateApiKey()
    {
        try
        {
            var result = await _mediator.Send(new RegenerateApiKeyCommand());
            return CreatedAtAction(nameof(GetApiKey), result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get API key usage statistics
    /// </summary>
    /// <returns>The usage statistics for the current user's API key</returns>
    [HttpGet("usage")]
    [SwaggerOperation(Summary = "Get API key usage statistics")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the usage statistics", typeof(ApiKeyUsageDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No API key exists for the user")]
    public async Task<IActionResult> GetApiKeyUsage()
    {
        try
        {
            var result = await _mediator.Send(new GetApiKeyUsageQuery());
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("No API key exists for this user.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
