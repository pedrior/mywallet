using MyWallet.Features.Wallets.Shared.Errors;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Shared.Security;

public sealed class WalletOwnerRequirement(Ulid userId, Ulid walletId) : ResourceRequirement
{
    public Ulid UserId => userId;

    public Ulid WalletId => walletId;

    public override Error ResourceNotFoundFallbackError => WalletErrors.NotFound;
}