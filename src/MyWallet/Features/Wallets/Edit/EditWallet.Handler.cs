using MyWallet.Domain;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Wallets.Edit;

public sealed class EditWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<EditWalletCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(EditWalletCommand command,
        CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetAsync(new(command.WalletId), cancellationToken);
        
        var name = WalletName.Create(command.Name).Value;
        var color = Color.Create(command.Color).Value;
        var currency = Currency.FromName(command.Currency, ignoreCase: true);
        
        return await wallet!.Edit(name, color, currency)
            .ThenDoAsync(_ => walletRepository.UpdateAsync(wallet, cancellationToken));
    }
}