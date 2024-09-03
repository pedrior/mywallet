namespace MyWallet.Features.Transactions.Get;

public sealed record GetTransactionQuery : IQuery<GetTransactionResponse>, IHaveUser
{
    public required Ulid TransactionId { get; init; }
    
    public Ulid UserId { get; set; }
}