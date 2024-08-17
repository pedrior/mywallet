using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Edit;

public sealed class EditWalletAuthorizer : IAuthorizer<EditWalletCommand>
{
    public IEnumerable<IRequirement> GetRequirements(EditWalletCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
    }
}