using MyWallet.Shared.Identity;
using MyWallet.Shared.Security;

namespace MyWallet.Shared.Behaviors;

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    IUserContext userContext,
    IServiceProvider services,
    IAuthorizer<TRequest>? authorizer = null) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (authorizer is null)
        {
            return await next();
        }

        if (!await userContext.IsAuthenticatedAsync(cancellationToken))
        {
            return (dynamic)Error.Unauthorized(
                code: string.Empty,
                description: "User is not authenticated");
        }

        foreach (var requirement in authorizer.GetRequirements(request))
        {
            var result = await FindRequirementHandler(requirement.GetType())
                .HandleAsync(requirement, cancellationToken);

            if (result.IsError)
            {
                return (dynamic)result.Errors;
            }
        }

        return await next();
    }

    private IRequirementHandler FindRequirementHandler(Type type)
    {
        var handlerType = typeof(IRequirementHandler<>)
            .MakeGenericType(type);

        var handler = services.GetRequiredService(handlerType) as IRequirementHandler;
        return handler ?? throw new InvalidOperationException(
            $"No authorization requirement handler found for type '{type.Name}'");
    }
}