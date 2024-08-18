namespace MyWallet.Features.Transactions.Shared;

public static class TransactionErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "transaction.not_found",
        description: "The transaction does not exist.");

    public static readonly Error WalletNotFound = Error.NotFound(
        code: "wallet.not_found",
        description: "The wallet where the transaction belongs does not exist.");
    
    public static readonly Error CategoryNotFound = Error.NotFound(
        code: "category.not_found",
        description: "The category of the transaction does not exist.");
}