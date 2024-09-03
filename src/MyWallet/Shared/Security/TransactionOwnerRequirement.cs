using MyWallet.Domain.Transactions;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Security;

public sealed class TransactionOwnerRequirement(Ulid userId, Ulid transactionId) : Requirement
{
    public Ulid UserId => userId;

    public Ulid TransactionId => transactionId;
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
            return requirement.Allow();
        }

        var walletId = await transactionRepository.GetWalletIdAsync(transactionId, cancellationToken);
        if (walletId.IsError)
        {
            return requirement.Allow();
        }

        return await walletRepository.IsOwnedByUserAsync(walletId.Value, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}