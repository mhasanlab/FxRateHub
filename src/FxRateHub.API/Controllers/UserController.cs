using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FxRateHub.Application.Features.User.Queries;
using FxRateHub.Application.Features.User.Commands;
using FxRateHub.Application.Features.User.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace FxRateHub.API.Controllers;

/// <summary>
/// Controller for user-related operations requiring authentication.
/// </summary>
[ApiController]
[Route("api/user")]
[Authorize]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the current user's profile information.
    /// </summary>
    /// <returns>The user's profile data including id, email, full name, and creation date.</returns>
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var result = await _mediator.Send(new GetUserProfileQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Updates the current user's profile.
    /// </summary>
    /// <param name="command">The profile update command containing the new full name.</param>
    /// <returns>The updated user's profile data.</returns>
    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
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
    /// Gets the complete dashboard data for the current user.
    /// </summary>
    /// <returns>The user's dashboard data including exchange rates and account information.</returns>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDashboardDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var result = await _mediator.Send(new GetUserDashboardQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
