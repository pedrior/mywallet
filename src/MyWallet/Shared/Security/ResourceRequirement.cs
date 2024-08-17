namespace MyWallet.Shared.Security;

public abstract class ResourceRequirement : Requirement
{
    protected abstract string ResourceName { get; }

    public Error ResourceNotFound() => Error.NotFound(description: $"{ResourceName} not found.");
}