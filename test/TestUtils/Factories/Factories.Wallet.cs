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
        
        public static MyWallet.Domain.Wallets.Wallet CreateDefault2(
            WalletId? id = null,
            UserId? userId = null,
            WalletName? name = null,
            Color? color = null,
            Currency? currency = null)
        {
            return MyWallet.Domain.Wallets.Wallet.Create(
                id ?? Constants.Constants.Wallet.Id2,
                userId ?? Constants.Constants.User.Id,
                name ?? Constants.Constants.Wallet.Name2,
                color ?? Constants.Constants.Wallet.Color2,
                currency ?? Constants.Constants.Wallet.Currency2);
        }
    }
}