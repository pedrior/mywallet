namespace MyWallet.Features.Transactions.Shared;

public static class TransactionErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "transaction.not_found",
        description: "The transaction does not exist.");
}