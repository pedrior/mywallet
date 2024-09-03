namespace MyWallet.Features.Transactions.Edit;

public sealed record EditTransactionCommand : ICommand<Updated>, IHaveUser
{
    public required Ulid TransactionId { get; init; }
    
    public Ulid UserId { get; set; }
    
    public Ulid? WalletId { get; init; } 
    
    public Ulid? CategoryId { get; init; }
    
    public string? Name { get; init; }
    
    public decimal? Amount { get; init; }
    
    public string? Currency { get; init; }
    
    public DateOnly? Date { get; init; }
}