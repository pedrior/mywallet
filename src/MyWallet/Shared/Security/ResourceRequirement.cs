namespace MyWallet.Shared.Security;

public abstract class ResourceRequirement : Requirement
{
    public virtual Error ResourceNotFoundFallbackError { get; } = Error.NotFound(
        code: "resource_not_found", description: "The requested resource was not found.");
}