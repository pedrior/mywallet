using MyWallet.Features.Wallets.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Delete;

public sealed class DeleteWalletAuthorizer : IAuthorizer<DeleteWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(DeleteWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}