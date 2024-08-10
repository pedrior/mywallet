using MyWallet.Features.Wallets.Errors;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets.Security;

public sealed class WalletOwnerRequirement(Ulid userId, Ulid walletId) : ResourceRequirement
{
    public Ulid UserId => userId;

    public Ulid WalletId => walletId;

    public override Error ResourceNotFoundFallbackError => WalletErrors.NotFound;

    protected override string ForbiddenDescription => "You are not the owner of this wallet.";
}