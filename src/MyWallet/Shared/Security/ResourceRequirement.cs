namespace MyWallet.Shared.Security;

public abstract class ResourceRequirement : Requirement
{
    public abstract Error ResourceNotFoundFallbackError { get; }
}