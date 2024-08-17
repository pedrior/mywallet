namespace MyWallet.Features.Transactions.Create;

public sealed record CreateTransactionRequest
{
    public required Ulid WalletId { get; init; }
    
    public required Ulid CategoryId { get; init; }
    
    public required string Type { get; init; }
    
    public required string Name { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required string Currency { get; init; }
    
    public required DateOnly Date { get; init; }
    
    public CreateTransactionCommand ToCommand() => new()
    {
        WalletId = WalletId,
        CategoryId = CategoryId,
        Type = Type,
        Name = Name,
        Amount = Amount,
        Currency = Currency,
        Date = Date
    };
}