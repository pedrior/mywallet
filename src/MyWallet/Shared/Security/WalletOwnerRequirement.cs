using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.Shared.Security;

public sealed class WalletOwnerRequirement(Ulid userId, Ulid walletId) : ResourceRequirement
{
    public Ulid UserId => userId;

    public Ulid WalletId => walletId;

    protected override string ResourceName => "Wallet";

    protected override string ForbiddenDescription => "You are not the owner of this wallet.";
}

public sealed class WalletOwnerRequirementHandler(IWalletRepository walletRepository)
    : IRequirementHandler<WalletOwnerRequirement>
{
    public async Task<ErrorOr<Success>> HandleAsync(WalletOwnerRequirement requirement,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(requirement.UserId);
        var walletId = new WalletId(requirement.WalletId);

        if (!await walletRepository.ExistsAsync(walletId, cancellationToken))
        {
            return requirement.ResourceNotFound();
        }

        return await walletRepository.IsOwnedByUserAsync(walletId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}