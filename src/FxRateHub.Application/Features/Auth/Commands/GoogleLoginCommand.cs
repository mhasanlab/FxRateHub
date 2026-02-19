using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using FxRateHub.Application.Common.Interfaces;
using FxRateHub.Application.Common.Models;
using FxRateHub.Application.Features.Auth.DTOs;

namespace FxRateHub.Application.Features.Auth.Commands;

/// <summary>
/// Command for Google OAuth login.
/// </summary>
public record GoogleLoginCommand : IRequest<Result<AuthResponseDto>>
{
    /// <summary>
    /// The Google ID token from the client.
    /// </summary>
    public string IdToken { get; init; } = string.Empty;
}

/// <summary>
/// Validator for the GoogleLoginCommand.
/// </summary>
public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        // IdToken rules
        RuleFor(x => x.IdToken)
            .NotEmpty()
            .WithMessage("Google ID token is required");
    }
}
