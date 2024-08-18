using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Security;

public sealed class TransactionOwnerRequirement(Ulid userId, Ulid transactionId) : ResourceRequirement
{
    public Ulid UserId => userId;

    public Ulid TransactionId => transactionId;

    protected override string ResourceName => "Transaction";

    protected override string ForbiddenDescription => "You are not the owner of this transaction.";
}

public sealed class TransactionOwnerRequirementHandler(
    ITransactionRepository transactionRepository,
    IWalletRepository walletRepository)
    : IRequirementHandler<TransactionOwnerRequirement>
{
    public async Task<ErrorOr<Success>> HandleAsync(TransactionOwnerRequirement requirement,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(requirement.UserId);
        var transactionId = new TransactionId(requirement.TransactionId);

        if (!await transactionRepository.ExistsAsync(transactionId, cancellationToken))
        {
            return requirement.ResourceNotFound();
        }

        var walletId = await transactionRepository.GetWalletIdAsync(transactionId, cancellationToken);
        if (walletId is null)
        {
            return requirement.Forbid();
        }

        return await walletRepository.IsOwnedByUserAsync(walletId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}