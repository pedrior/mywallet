using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Unarchive;

public sealed class UnarchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<UnarchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(UnarchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var walletId = new WalletId(command.WalletId);
        var wallet = await walletRepository.GetAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Shared.WalletErrors.NotFound;
        }
        
        return await wallet.Unarchive()
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}