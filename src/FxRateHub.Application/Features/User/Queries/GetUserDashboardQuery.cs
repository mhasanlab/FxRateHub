using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ApiKeys.DTOs;
using FxRateHub.Application.Features.ApiKeys.Queries;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using FxRateHub.Application.Features.ExchangeRates.Queries;
using FxRateHub.Application.Features.User.DTOs;

namespace FxRateHub.Application.Features.User.Queries;

/// <summary>
/// Query to retrieve the current user's dashboard information.
/// </summary>
public record GetUserDashboardQuery : IRequest<UserDashboardDto>
{
}

/// <summary>
/// Handler for the GetUserDashboardQuery.
/// </summary>
public class GetUserDashboardQueryHandler : IRequestHandler<GetUserDashboardQuery, UserDashboardDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public GetUserDashboardQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<UserDashboardDto> Handle(
        GetUserDashboardQuery request,
        CancellationToken cancellationToken)
    {
        // Check if user is authenticated
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == null)
        {
            throw new ForbiddenException("User not authenticated");
        }

        // Get user profile
        var profile = await _mediator.Send(new GetUserProfileQuery(), cancellationToken);

        // Get API key info if exists
        var apiKey = await _mediator.Send(new GetApiKeyQuery(), cancellationToken);

        // Get usage stats if API key exists
        ApiKeyUsageDto? usage = null;
        if (apiKey != null)
        {
            usage = await _mediator.Send(new GetApiKeyUsageQuery(), cancellationToken);
        }

        // Get top 10 exchange rates (using USD as default base currency)
        var topRates = await GetTopExchangeRatesAsync(cancellationToken);

        return new UserDashboardDto(
            Profile: profile,
            ApiKey: apiKey,
            Usage: usage,
            TopCurrencies: topRates
        );
    }

    /// <summary>
    /// Gets the top 10 exchange rates for USD as the base currency.
    /// </summary>
    private async Task<List<ExchangeRateDto>> GetTopExchangeRatesAsync(CancellationToken cancellationToken)
    {
        const string defaultBaseCurrency = "USD";
        const int topCount = 10;

        // Get all rates for USD base currency
        var rates = await _dbContext.ExchangeRates
            .Where(r => r.BaseCurrency.ToUpper() == defaultBaseCurrency)
            .OrderBy(r => r.TargetCurrency)
            .Take(topCount)
            .ToListAsync(cancellationToken);

        return rates.Select(r => new ExchangeRateDto(
            BaseCurrency: r.BaseCurrency,
            TargetCurrency: r.TargetCurrency,
            Rate: r.Rate,
            UpdatedAt: r.UpdatedAt
        )).ToList();
    }
}
