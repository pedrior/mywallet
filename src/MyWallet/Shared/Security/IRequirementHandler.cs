namespace MyWallet.Shared.Security;

public interface IRequirementHandler
{
    Task<ErrorOr<Success>> HandleAsync(object requirement, CancellationToken cancellationToken);
}

public interface IRequirementHandler<in TRequirement> : IRequirementHandler where TRequirement : IRequirement
{
    Task<ErrorOr<Success>> HandleAsync(TRequirement requirement, CancellationToken cancellationToken);

    Task<ErrorOr<Success>> IRequirementHandler.HandleAsync(object requirement, CancellationToken cancellationToken) =>
        HandleAsync((TRequirement)requirement, cancellationToken);
}