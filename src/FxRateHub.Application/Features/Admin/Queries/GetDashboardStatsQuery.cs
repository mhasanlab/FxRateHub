using MediatR;
using FxRateHub.Application.Features.Admin.DTOs;

namespace FxRateHub.Application.Features.Admin.Queries;

public record GetDashboardStatsQuery() : IRequest<DashboardStatsDto>;
