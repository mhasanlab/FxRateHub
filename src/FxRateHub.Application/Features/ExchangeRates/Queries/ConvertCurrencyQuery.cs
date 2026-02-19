using System;
using System.Threading;
using System.Threading.Tasks;
using FxRateHub.Application.Common.Exceptions;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Features.ExchangeRates.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FxRateHub.Application.Features.ExchangeRates.Queries;

public record ConvertCurrencyQuery(string FromCurrency, string ToCurrency, decimal Amount) : IRequest<ConversionResultDto>;

public class ConvertCurrencyQueryValidator : AbstractValidator<ConvertCurrencyQuery>
{
    public ConvertCurrencyQueryValidator()
    {
        RuleFor(x => x.FromCurrency).NotEmpty().Length(3);
        RuleFor(x => x.ToCurrency).NotEmpty().Length(3);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class ConvertCurrencyQueryHandler : IRequestHandler<ConvertCurrencyQuery, ConversionResultDto>
{
    private readonly IApplicationDbContext _context;
    
    public ConvertCurrencyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<ConversionResultDto> Handle(ConvertCurrencyQuery request, CancellationToken cancellationToken)
    {
        var fromCurrency = request.FromCurrency.ToUpper();
        var toCurrency = request.ToCurrency.ToUpper();
        var amount = request.Amount;
        
        // If same currency, return direct conversion
        if (fromCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return new ConversionResultDto(
                From: fromCurrency,
                To: toCurrency,
                Amount: amount,
                ConvertedAmount: amount,
                Rate: 1,
                Timestamp: DateTime.UtcNow
            );
        }
        
        // Try to find direct rate
        var directRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => 
                r.BaseCurrency.Equals(fromCurrency, StringComparison.OrdinalIgnoreCase) &&
                r.TargetCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        
        if (directRate != null)
        {
            var convertedAmount = amount * directRate.Rate;
            return new ConversionResultDto(
                From: fromCurrency,
                To: toCurrency,
                Amount: amount,
                ConvertedAmount: convertedAmount,
                Rate: directRate.Rate,
                Timestamp: directRate.UpdatedAt
            );
        }
        
        // Try reverse rate
        var reverseRate = await _context.ExchangeRates
            .FirstOrDefaultAsync(r => 
                r.BaseCurrency.Equals(toCurrency, StringComparison.OrdinalIgnoreCase) &&
                r.TargetCurrency.Equals(fromCurrency, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        
        if (reverseRate != null)
        {
            var convertedAmount = amount / reverseRate.Rate;
            return new ConversionResultDto(
                From: fromCurrency,
                To: toCurrency,
                Amount: amount,
                ConvertedAmount: convertedAmount,
                Rate: 1 / reverseRate.Rate,
                Timestamp: reverseRate.UpdatedAt
            );
        }
        
        throw new NotFoundException($"Cannot convert from '{fromCurrency}' to '{toCurrency}': exchange rate not found");
    }
}
