using MyWallet.Shared.Persistence;

namespace MyWallet.Shared.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IDbContext context, ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ITransactional
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Beginning transaction for {RequestName}", requestName);
        context.BeginTransaction();

        try
        {
            var response = await next();

            logger.LogInformation("Committing transaction for {RequestName}", requestName);
            context.CommitTransaction();

            return response;
        }
        catch
        {
            logger.LogInformation("Rolling back transaction for {RequestName}", requestName);

            context.RollbackTransaction();
            
            throw;
        }
    }
}