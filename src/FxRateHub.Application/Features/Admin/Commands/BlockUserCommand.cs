using System;
using MediatR;
using FxRateHub.Application.Common.Models;

namespace FxRateHub.Application.Features.Admin.Commands;

public record BlockUserCommand(Guid UserId, bool Block) : IRequest<Result<string>>;
