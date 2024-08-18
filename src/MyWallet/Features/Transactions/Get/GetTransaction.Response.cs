namespace MyWallet.Features.Transactions.Get;

public sealed record GetTransactionResponse
{
    public required Ulid Id { get; init; }
    
    public required Ulid WalletId { get; init; }
    
    public required Ulid CategoryId { get; init; }
    
    public required string CategoryName { get; init; }
    
    public required string Type { get; init; }
    
    public required string Name { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required string Currency { get; init; }
    
    public required DateOnly Date { get; init; }
    
    public required DateTimeOffset CreatedAt { get; init; }
    
    public required DateTimeOffset? UpdatedAt { get; init; }
}