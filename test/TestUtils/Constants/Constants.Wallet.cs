using MyWallet.Domain;
using MyWallet.Domain.Wallets.ValueObjects;

namespace TestUtils.Constants;

public static partial class Constants
{
    public static class Wallet
    {
        public static readonly WalletId Id = WalletId.New();
        public static readonly WalletName Name = WalletName.Create("NU Wallet").Value;
        public static readonly Color Color = Color.Create("#BA3F1D").Value;
    }
}