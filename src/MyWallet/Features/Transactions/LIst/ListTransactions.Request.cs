namespace MyWallet.Features.Transactions.List;

public sealed record ListTransactionsRequest
{
    public required Ulid WalletId { get; init; }

    public required DateOnly From { get; init; }

    public DateOnly? To { get; init; }
    
    public int? Page { get; init; }
    
    public int? Limit { get; init; }

    public ListTransactionsQuery ToQuery() => new()
    {
        WalletId = WalletId,
        From = From,
        To = To ?? DateOnly.MaxValue,
        Page = Page ?? 1,
        Limit = Limit ?? 10
    };
}