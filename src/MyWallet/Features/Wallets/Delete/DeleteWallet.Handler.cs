using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Delete;

public sealed class DeleteWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<DeleteWalletCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteWalletCommand commnad,
        CancellationToken cancellationToken)
    {
        var walletId = new WalletId(commnad.WalletId);

        if (!await walletRepository.ExistsAsync(walletId, cancellationToken))
        {
            return Shared.WalletErrors.NotFound;
        }

        await walletRepository.DeleteAsync(walletId, cancellationToken);
        return Result.Deleted;
    }
}