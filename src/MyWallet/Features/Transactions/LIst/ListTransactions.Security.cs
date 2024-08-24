using MyWallet.Shared.Security;

namespace MyWallet.Features.Transactions.List;

public sealed class ListTransactionsSecurity : IAuthorizer<ListTransactionsQuery>
{
    public IEnumerable<IRequirement> GetRequirements(ListTransactionsQuery query)
    {
        yield return new WalletOwnerRequirement(query.UserId, query.WalletId);
    }
}