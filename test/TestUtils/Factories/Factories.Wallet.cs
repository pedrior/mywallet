using MyWallet.Domain;
using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets.ValueObjects;

namespace TestUtils.Factories;

public static partial class Factories
{
    public static class Wallet
    {
        public static MyWallet.Domain.Wallets.Wallet CreateDefault(
            WalletId? id = null,
            UserId? userId = null,
            WalletName? name = null,
            Color? color = null,
            bool isArchived = false)
        {
            var wallet = MyWallet.Domain.Wallets.Wallet.Create(
                id ?? Constants.Constants.Wallet.Id,
                userId ?? Constants.Constants.User.Id,
                name ?? Constants.Constants.Wallet.Name,
                color ?? Constants.Constants.Wallet.Color);

            if (isArchived)
            {
                wallet.GetType()
                    .GetProperty(nameof(MyWallet.Domain.Wallets.Wallet.IsArchived))!
                    .SetValue(wallet, true);
            }

            return wallet;
        }
    }
}