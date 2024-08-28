using MyWallet.Domain.Transactions;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Delete;

public sealed class DeleteTransactionHandler(ITransactionRepository transactionRepository)
    : ICommandHandler<DeleteTransactionCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var transactionId = new TransactionId(command.TransactionId);
        if (!await transactionRepository.ExistsAsync(transactionId, cancellationToken))
        {
            return Shared.TransactionErrors.NotFound;
        }

        await transactionRepository.DeleteAsync(transactionId, cancellationToken);
        return Result.Deleted;
    }
}