namespace MyWallet.Features.Transactions.Edit;

public sealed record EditTransactionRequest
{
    public Ulid? WalletId { get; init; }

    public Ulid? CategoryId { get; init; }

    public string? Name { get; init; }

    public decimal? Amount { get; init; }

    public string? Currency { get; init; }

    public DateOnly? Date { get; init; }

    public EditTransactionCommand ToCommand(Ulid transactionId) => new()
    {
        TransactionId = transactionId,
        WalletId = WalletId,
        CategoryId = CategoryId,
        Name = Name,
        Amount = Amount,
        Currency = Currency,
        Date = Date
    };
}