using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Delete;

public sealed class DeleteWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<DeleteWalletCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteWalletCommand commnad,
        CancellationToken cancellationToken)
    {
        await walletRepository.DeleteAsync(new WalletId(commnad.WalletId), cancellationToken);
        return Result.Deleted;
    }
}