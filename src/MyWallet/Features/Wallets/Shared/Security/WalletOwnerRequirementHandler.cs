using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Shared.Security;

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
            return requirement.ResourceNotFoundFallbackError;
        }

        return await walletRepository.IsOwnedByUserAsync(walletId, userId, cancellationToken)
            ? requirement.Allow()
            : requirement.Forbid();
    }
}