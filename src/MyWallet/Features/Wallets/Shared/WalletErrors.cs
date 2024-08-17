namespace MyWallet.Features.Wallets.Shared;

public static class WalletErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "wallet.not_found",
        description: "Wallet not found.");
}