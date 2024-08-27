namespace MyWallet.Shared.Security;

public sealed class AlwaysAllowRequirement : Requirement;

public sealed class AlwaysAllowRequirementHandler : IRequirementHandler<AlwaysAllowRequirement>
{
    public Task<ErrorOr<Success>> HandleAsync(AlwaysAllowRequirement requirement, 
        CancellationToken cancellationToken)
    {
        return Task.FromResult<ErrorOr<Success>>(requirement.Allow());
    }
}