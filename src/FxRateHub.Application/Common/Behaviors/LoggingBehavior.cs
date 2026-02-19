using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FxRateHub.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Processing request {RequestName}", requestName);

        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            var response = await next();
            
            stopwatch.Stop();
            
            _logger.LogInformation("Completed request {RequestName} in {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request {RequestName}: {ErrorMessage}", 
                requestName, ex.Message);
            throw;
        }
    }
}
