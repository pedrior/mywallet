using MyWallet.Domain;
using MyWallet.Domain.Users;
using MyWallet.Domain.Wallets;

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
            Currency? currency = null)
        {
            return MyWallet.Domain.Wallets.Wallet.Create(
                id ?? Constants.Constants.Wallet.Id,
                userId ?? Constants.Constants.User.Id,
                name ?? Constants.Constants.Wallet.Name,
                color ?? Constants.Constants.Wallet.Color,
                currency ?? Constants.Constants.Wallet.Currency);
        }
    }
}