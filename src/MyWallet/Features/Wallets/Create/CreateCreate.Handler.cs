using MyWallet.Domain;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

namespace MyWallet.Features.Wallets.Create;

public sealed class CreateWalletHandler(IWalletRepository walletRepository)
    : ICommandHandler<CreateWalletCommand, WalletId>
{
    public async Task<ErrorOr<WalletId>> Handle(CreateWalletCommand command,
        CancellationToken cancellationToken)
    {
        var name = WalletName.Create(command.Name).Value;
        var color = Color.Create(command.Color).Value;
        var currency = Currency.FromName(command.Currency, ignoreCase: true);

        var wallet = Wallet.Create(
            WalletId.New(),
            new UserId(command.UserId),
            name,
            color,
            currency);

        await walletRepository.AddAsync(wallet, cancellationToken);

        return wallet.Id;
    }
}