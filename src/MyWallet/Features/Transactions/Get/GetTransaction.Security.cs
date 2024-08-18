using MyWallet.Shared.Security;

namespace MyWallet.Features.Transactions.Get;

public sealed class GetTransactionSecurity : IAuthorizer<GetTransactionQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetTransactionQuery query)
    {
        yield return new TransactionOwnerRequirement(query.UserId, query.TransactionId);
    }
}