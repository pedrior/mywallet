using MyWallet.Features.Wallets.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Get;

public sealed class GetWalletAuthorizer : IAuthorizer<GetWalletQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetWalletQuery query)
    {
        yield return new WalletOwnerRequirement(query.UserId, query.WalletId);
    }
}