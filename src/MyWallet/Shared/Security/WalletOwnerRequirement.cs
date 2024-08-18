using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Security;

public sealed class WalletOwnerRequirement(Ulid userId, Ulid walletId) : Requirement
{
    public Ulid UserId => userId;

    public Ulid WalletId => walletId;
}

public sealed class WalletOwnerRequirementHandler(IWalletRepository walletRepository)
    : IRequirementHandler<WalletOwnerRequirement>
{
    public async Task<ErrorOr<Success>> HandleAsync(WalletOwnerRequirement requirement,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(requirement.UserId);
        var walletId = new WalletId(requirement.WalletId);

        return !await walletRepository.ExistsAsync(walletId, cancellationToken)
               || await walletRepository.IsOwnedByUserAsync(walletId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}