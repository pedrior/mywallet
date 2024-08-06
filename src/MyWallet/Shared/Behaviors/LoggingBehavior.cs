using MediatR.Pipeline;

namespace MyWallet.Shared.Behaviors;

public sealed class LoggingBehavior<TRequest>(ILogger<TRequest> logger)
    : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {RequestName} ({@Request})", typeof(TRequest).Name, request);

        await Task.CompletedTask;
    }
}