using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Archive;

public sealed class ArchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<ArchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ArchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var walletId = new WalletId(command.WalletId);
        var wallet = await walletRepository.GetAsync(walletId, cancellationToken);
        
        return await wallet!.Archive()
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}