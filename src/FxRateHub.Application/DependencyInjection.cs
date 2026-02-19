using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FxRateHub.Application.Common.Behaviors;

namespace FxRateHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR from the Application assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // Register all validators from the Application assembly using FluentValidation DI extensions
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register ValidationBehavior as IPipelineBehavior (transient)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // Register LoggingBehavior as IPipelineBehavior (transient)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        return services;
    }
}
