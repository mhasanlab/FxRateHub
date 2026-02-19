using System.Collections.Generic;
using MediatR;
using FxRateHub.Application.Features.Admin.DTOs;

namespace FxRateHub.Application.Features.Admin.Queries;

public record GetAllSettingsQuery() : IRequest<List<AppSettingDto>>;
