using MediatR;
using FxRateHub.Application.Features.Auth.DTOs;

namespace FxRateHub.Application.Features.Auth.Commands.GoogleLogin;

public class GoogleLoginCommand : IRequest<AuthResponseDto>
{
    public string IdToken { get; set; } = string.Empty;
}
