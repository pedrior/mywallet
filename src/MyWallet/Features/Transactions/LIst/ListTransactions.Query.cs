namespace MyWallet.Features.Transactions.List;

public sealed record ListTransactionsQuery : IQuery<ListTransactionsResponse>, IHaveUser
{
    public required Ulid WalletId { get; init; }

    public required DateOnly From { get; init; }

    public required DateOnly To { get; init; }
    
    public required int Page { get; init; }
    
    public required int Limit { get; init; }
    
    public Ulid UserId { get; set; }
    
    public int Offset => (Page - 1) * Limit;
}