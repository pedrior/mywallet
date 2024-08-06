namespace MyWallet.Shared.Security;

public interface IAuthorizer<in TSubject> where TSubject : class
{
    IEnumerable<IRequirement> GetRequirements(TSubject subject);
}