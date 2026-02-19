using MediatR;
using FxRateHub.Application.Common.Models;

namespace FxRateHub.Application.Features.Admin.Commands;

public record UpdateSettingCommand(string Key, string Value) : IRequest<Result<string>>;
