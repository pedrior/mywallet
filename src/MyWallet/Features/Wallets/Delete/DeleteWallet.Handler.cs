using MyWallet.Domain.Wallets;

namespace MyWallet.Features.Wallets.Delete;

public sealed class DeleteWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<DeleteWalletCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(
            new WalletId(command.WalletId), 
            cancellationToken);

        return await wallet
            .ThenDo(w => w.Delete())
            .ThenDoAsync(w => walletRepository.UpdateAsync(w, cancellationToken))
            .Then(_ => Result.Deleted);
    }
}