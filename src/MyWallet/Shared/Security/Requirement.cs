namespace MyWallet.Shared.Security;

public abstract class Requirement : IRequirement
{
    public virtual Error Forbidden { get; } = Error.Forbidden(
        code: "forbidden",
        description: "You are not authorized to perform this action.");
}