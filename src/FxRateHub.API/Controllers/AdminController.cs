using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using FxRateHub.Application.Features.Admin.Queries;
using FxRateHub.Application.Features.Admin.Commands;
using FxRateHub.Application.Features.Admin.DTOs;
using FxRateHub.Application.Features.ExchangeRates.Commands;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using FxRateHub.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace FxRateHub.API.Controllers;

/// <summary>
/// Controller for admin-only operations including user management, dashboard statistics, settings, and rate synchronization.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Policy = "AdminOnly")]
[Produces("application/json")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paginated list of all users.
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="search">Optional search term to filter by email or full name</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<AdminUserDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? search = null)
    {
        try
        {
            var result = await _mediator.Send(new GetAllUsersQuery(page, pageSize, search));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets details of a specific user by ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>User details</returns>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminUserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
            var result = await _mediator.Send(new GetUserByIdQuery(userId));
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.Error);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Blocks or unblocks a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="request">The block request containing the block status</param>
    /// <returns>Success message</returns>
    [HttpPut("users/{userId:guid}/block")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> BlockUser(Guid userId, [FromBody] BlockUserRequest request)
    {
        try
        {
            var result = await _mediator.Send(new BlockUserCommand(userId, request.Block));
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Error);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets dashboard statistics.
    /// </summary>
    /// <returns>Dashboard statistics including user counts, API usage, and sync information</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DashboardStatsDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets all application settings.
    /// </summary>
    /// <returns>List of application settings</returns>
    [HttpGet("settings")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AppSettingDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllSettings()
    {
        try
        {
            var result = await _mediator.Send(new GetAllSettingsQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an application setting.
    /// </summary>
    /// <param name="key">The setting key</param>
    /// <param name="request">The update request containing the new value</param>
    /// <returns>Success message</returns>
    [HttpPut("settings/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingRequest request)
    {
        try
        {
            var result = await _mediator.Send(new UpdateSettingCommand(key, request.Value));
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Error);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Manually triggers FX rate synchronization.
    /// </summary>
    /// <returns>Sync result containing updated currencies count and status</returns>
    [HttpPost("sync-rates")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SyncResultDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SyncExchangeRates()
    {
        try
        {
            var result = await _mediator.Send(new SyncExchangeRatesCommand());
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets recent synchronization logs.
    /// </summary>
    /// <param name="count">Number of logs to return (default: 10)</param>
    /// <returns>List of recent sync logs</returns>
    [HttpGet("sync-logs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RecentSyncDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSyncLogs([FromQuery] int count = 10)
    {
        try
        {
            var result = await _mediator.Send(new GetSyncLogsQuery(count));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

/// <summary>
/// Request model for blocking/unblocking a user.
/// </summary>
public class BlockUserRequest
{
    /// <summary>
    /// True to block the user, false to unblock.
    /// </summary>
    public bool Block { get; set; }
}

/// <summary>
/// Request model for updating a setting.
/// </summary>
public class UpdateSettingRequest
{
    /// <summary>
    /// The new value for the setting.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
