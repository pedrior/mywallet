using MyWallet.Domain.Transactions;

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
            return TransactionErrors.NotFound;
        }

        await transactionRepository.DeleteAsync(transactionId, cancellationToken);
        return Result.Deleted;
    }
}