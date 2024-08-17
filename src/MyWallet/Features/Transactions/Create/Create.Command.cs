using MyWallet.Shared.Features;

namespace MyWallet.Features.Transactions.Create;

public sealed record CreateTransactionCommand : ICommand<Ulid>, IHaveUser
{
    public required Ulid WalletId { get; init; }
    
    public required Ulid CategoryId { get; init; }
    
    public required string Type { get; init; }
    
    public required string Name { get; init; }
    
    public required decimal Amount { get; init; }
    
    public required string Currency { get; init; }
    
    public required DateOnly Date { get; init; }
    
    public Ulid UserId { get; set; }
}