using MyWallet.Shared.Security;

namespace MyWallet.Features.Transactions.Edit;

public sealed class EditTransactionSecurity : IAuthorizer<EditTransactionCommand>
{
    public IEnumerable<IRequirement> GetRequirements(EditTransactionCommand command)
    {
        yield return new TransactionOwnerRequirement(command.UserId, command.TransactionId);

        yield return command.WalletId is null
            ? new AlwaysAllowRequirement()
            : new WalletOwnerRequirement(command.UserId, command.WalletId!.Value);
        
        yield return command.CategoryId is null
            ? new AlwaysAllowRequirement()
            : new CategoryOwnerRequirement(command.UserId, command.CategoryId!.Value);
    }
}