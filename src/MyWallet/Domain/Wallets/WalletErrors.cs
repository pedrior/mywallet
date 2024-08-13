namespace MyWallet.Domain.Wallets;

public static class WalletErrors
{
    public static readonly Error WalletIsArchived = Error.Failure(
        code: "wallet.archived",
        description: "The Wallet is archived and cannot be modified.");
}