namespace MyWallet.Features.Transactions.Delete;

public sealed record DeleteTransactionCommand(Ulid TransactionId) : ICommand<Deleted>, IHaveUser
{
    public Ulid UserId { get; set; }
}