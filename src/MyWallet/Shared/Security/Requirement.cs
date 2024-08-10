namespace MyWallet.Shared.Security;

public abstract class Requirement : IRequirement
{
    protected virtual string ForbiddenDescription => "You are not authorized to perform this action.";

    public Success Allow() => Result.Success;
    
    public Error Forbid() => Error.Forbidden(code: "forbidden", description: ForbiddenDescription);
}