using MyWallet.Domain;
using MyWallet.Domain.Wallets;

namespace TestUtils.Constants;

public static partial class Constants
{
    public static class Wallet
    {
        public static readonly WalletId Id = WalletId.New();
        public static readonly WalletName Name = WalletName.Create("NU Wallet").Value;
        public static readonly Color Color = Color.Create("#BA3F1D").Value;
        public static readonly Currency Currency = Currency.BRL;
        
        public static readonly WalletName Name2 = WalletName.Create("BB Wallet").Value;
    }
}