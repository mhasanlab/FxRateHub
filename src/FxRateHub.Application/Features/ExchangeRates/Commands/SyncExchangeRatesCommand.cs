using FxRateHub.Application.Features.ExchangeRates.Dtos;
using MediatR;

namespace FxRateHub.Application.Features.ExchangeRates.Commands;

public record SyncExchangeRatesCommand : IRequest<SyncResultDto>;
