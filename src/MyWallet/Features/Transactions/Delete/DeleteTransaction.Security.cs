using MyWallet.Shared.Security;

namespace MyWallet.Features.Transactions.Delete;

public sealed class DeleteTransactionSecurity : IAuthorizer<DeleteTransactionCommand>
{
    public IEnumerable<IRequirement> GetRequirements(DeleteTransactionCommand command)
    {
        yield return new TransactionOwnerRequirement(command.UserId, command.TransactionId);
    }
}