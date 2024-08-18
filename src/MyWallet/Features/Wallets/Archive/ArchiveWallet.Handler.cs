using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Archive;

public sealed class ArchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<ArchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ArchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(new WalletId(command.WalletId), 
            cancellationToken);

        if (wallet is null)
        {
            return Shared.WalletErrors.NotFound;
        }
        
        return await wallet.Archive()
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}