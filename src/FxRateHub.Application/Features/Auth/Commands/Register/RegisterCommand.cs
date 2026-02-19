using MediatR;
using FxRateHub.Application.Features.Auth.DTOs;

namespace FxRateHub.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
