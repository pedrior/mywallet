using MediatR.Pipeline;
using MyWallet.Shared.Identity;

namespace MyWallet.Shared.Behaviors;

public sealed class CurrentUserBehavior<TRequest>(IUserContext userContext)
    : IRequestPreProcessor<TRequest> where TRequest : IHaveUser
{
    public Task Process(TRequest request, CancellationToken _)
    {
        request.UserId = userContext.Id;
        return Task.CompletedTask;
    }
}