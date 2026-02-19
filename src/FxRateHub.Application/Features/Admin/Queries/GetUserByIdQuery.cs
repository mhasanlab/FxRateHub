using System;
using MediatR;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Admin.DTOs;

namespace FxRateHub.Application.Features.Admin.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<Result<AdminUserDto>>;
