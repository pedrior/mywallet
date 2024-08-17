using MyWallet.Features.Categories.Shared.Security;
using MyWallet.Features.Wallets.Shared.Security;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Transactions.Create;

public sealed class CreateTransactionAuthorizer : IAuthorizer<CreateTransactionCommand>
{
    public IEnumerable<IRequirement> GetRequirements(CreateTransactionCommand command)
    {
        yield return new WalletOwnerRequirement(command.UserId, command.WalletId);
        yield return new CategoryOwnerRequirement(command.UserId, command.CategoryId);
    }
}