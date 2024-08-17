using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Unarchive;

public sealed class UnarchiveWalletAuthorizer : IAuthorizer<UnarchiveWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(UnarchiveWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}