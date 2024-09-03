namespace MyWallet.Domain.Transactions;

public static class TransactionErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "transaction.not_found",
        description: "The Transaction does not exist.");

    public static readonly Error WalletNotFound = Error.NotFound(
        code: "transaction.wallet_not_found",
        description: "The Transaction's Wallet does not exist.");
    
    public static readonly Error AmountIsZero = Error.Failure(
        code: "transaction.amount_is_zero",
        description: "Amount must be greater than zero.");

    public static readonly Error WalletIsArchived = Error.Failure(
        code: "transaction.wallet_is_archived",
        description: "Wallet is archived and cannot be used for transactions.");

    public static readonly Error CurrencyMismatch = Error.Failure(
        code: "transaction.currency_mismatch",
        description: "Wallet currency does not match transaction currency.");

    public static readonly Error CategoryTypeMismatch = Error.Failure(
        code: "transaction.category_type_mismatch",
        description: "Category type does not match transaction type.");
}