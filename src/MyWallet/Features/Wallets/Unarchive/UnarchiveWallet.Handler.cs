using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Unarchive;

public sealed class UnarchiveWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<UnarchiveWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(UnarchiveWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(
            new WalletId(command.WalletId), 
            cancellationToken);

        return await wallet
            .ThenDoOrFail(w => w.Unarchive())
            .ThenDoAsync(w => walletRepository.UpdateAsync(w, cancellationToken))
            .Then(_ => Result.Success);
    }
}