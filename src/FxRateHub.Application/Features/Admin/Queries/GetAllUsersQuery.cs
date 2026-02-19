using MediatR;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Admin.DTOs;

namespace FxRateHub.Application.Features.Admin.Queries;

public record GetAllUsersQuery(int Page = 1, int PageSize = 10, string? Search = null) : IRequest<PaginatedList<AdminUserDto>>;
