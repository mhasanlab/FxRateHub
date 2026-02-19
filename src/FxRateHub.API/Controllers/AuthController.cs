using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FxRateHub.Application.Features.Auth.Commands.Register;
using FxRateHub.Application.Features.Auth.Commands.Login;
using FxRateHub.Application.Features.Auth.Commands.GoogleLogin;
using FxRateHub.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Http;

namespace FxRateHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user with the provided credentials.
    /// </summary>
    /// <param name="command">The registration command containing user details.</param>
    /// <returns>An authentication response containing the JWT token and user information.</returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Authenticates a user with their email and password.
    /// </summary>
    /// <param name="command">The login command containing user credentials.</param>
    /// <returns>An authentication response containing the JWT token and user information.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Authenticates a user using their Google ID token.
    /// </summary>
    /// <param name="command">The Google login command containing the ID token.</param>
    /// <returns>An authentication response containing the JWT token and user information.</returns>
    [HttpPost("google")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
