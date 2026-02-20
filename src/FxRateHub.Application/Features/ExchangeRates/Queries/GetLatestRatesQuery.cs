using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.ExchangeRates.Queries;

public record GetLatestRatesQuery(string BaseCurrency) : IRequest<ExchangeRatesResponseDto>;

public class GetLatestRatesQueryHandler : IRequestHandler<GetLatestRatesQuery, ExchangeRatesResponseDto>
{
    private readonly IApplicationDbContext _context;
    
    public GetLatestRatesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ExchangeRatesResponseDto> Handle(GetLatestRatesQuery request, CancellationToken cancellationToken)
    {
        var baseCurrency = request.BaseCurrency.ToUpper();
        
        // Get all rates for the base currency from database
        var rates = await _context.ExchangeRates
            .Where(r => r.BaseCurrency.ToUpper() == baseCurrency)
            .ToDictionaryAsync(r => r.TargetCurrency, r => r.Rate, cancellationToken);
        
        if (!rates.Any())
        {
            throw new NotFoundException($"No rates found for base currency '{baseCurrency}'. Please sync exchange rates first.");
        }
        
        return new ExchangeRatesResponseDto(
            BaseCurrency: baseCurrency,
            Timestamp: DateTime.UtcNow,
            Rates: rates
        );
    }
}
