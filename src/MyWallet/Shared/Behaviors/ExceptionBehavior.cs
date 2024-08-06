namespace MyWallet.Shared.Behaviors;

public sealed class ExceptionBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while handling {RequestName} with request {@Request}",
                typeof(TRequest).Name,
                request);

            return (dynamic)Error.Unexpected(
                code: string.Empty,
                description: "An internal error occurred");
        }
    }
}