using System;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.ExchangeRates.Queries;

public record GetRateByCodeQuery(string BaseCurrency, string TargetCurrency) : IRequest<ExchangeRateDto>;

public class GetRateByCodeQueryHandler : IRequestHandler<GetRateByCodeQuery, ExchangeRateDto>
{
    private readonly IApplicationDbContext _context;
    
    public GetRateByCodeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ExchangeRateDto> Handle(GetRateByCodeQuery request, CancellationToken cancellationToken)
    {
        var baseCurrency = request.BaseCurrency.ToUpper();
        var targetCurrency = request.TargetCurrency.ToUpper();
        
        var rate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => 
                r.BaseCurrency.Equals(baseCurrency, StringComparison.OrdinalIgnoreCase) &&
                r.TargetCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        
        if (rate == null)
        {
            throw new NotFoundException($"Exchange rate from '{baseCurrency}' to '{targetCurrency}' not found");
        }
        
        return new ExchangeRateDto(
            BaseCurrency: baseCurrency,
            TargetCurrency: targetCurrency,
            Rate: rate.Rate,
            UpdatedAt: rate.UpdatedAt
        );
    }
}
