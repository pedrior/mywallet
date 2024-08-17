using MyWallet.Features.Wallets.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Archive;

public sealed class ArchiveWalletAuthorizer : IAuthorizer<ArchiveWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(ArchiveWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}