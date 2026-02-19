using MediatR;
using FxRateHub.Application.Features.Auth.DTOs;

namespace FxRateHub.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
