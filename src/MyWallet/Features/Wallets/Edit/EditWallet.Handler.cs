using MyWallet.Domain;
using MyWallet.Domain.Wallets;

namespace MyWallet.Features.Wallets.Edit;

public sealed class EditWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<EditWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(EditWalletCommand command,
        CancellationToken cancellationToken)
    {
        var name = WalletName.Create(command.Name).Value;
        var color = Color.Create(command.Color).Value;
        var currency = Currency.FromName(command.Currency, ignoreCase: true);

        var wallet = await walletRepository.GetAsync(
            new WalletId(command.WalletId),
            cancellationToken);

        return await wallet
            .ThenDoOrFail(w => w.Edit(name, color, currency))
            .ThenDoAsync(w => walletRepository.UpdateAsync(w, cancellationToken))
            .Then(_ => Result.Success);
    }
}