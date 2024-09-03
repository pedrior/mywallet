using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Archive;

public sealed class ArchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<ArchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(ArchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(
            new WalletId(command.WalletId),
            cancellationToken);

        return await wallet
            .ThenDoOrFail(w => w.Archive())
            .ThenDoAsync(w => walletRepository.UpdateAsync(w, cancellationToken))
            .Then(_ => Result.Success);
    }
}