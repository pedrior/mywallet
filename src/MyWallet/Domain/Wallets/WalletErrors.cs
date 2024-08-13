namespace MyWallet.Domain.Wallets;

public static class WalletErrors
{
    public static readonly Error AlreadyArchived = Error.Failure(
        code: "wallet.already_archived",
        description: "The Wallet is already archived.");

    public static readonly Error WalletIsArchived = Error.Failure(
        code: "wallet.archived",
        description: "The Wallet is archived and cannot be modified.");
}