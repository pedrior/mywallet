namespace MyWallet.Domain.Transactions;

public sealed class TransactionId(Ulid id) : EntityId(id)
{
    public static TransactionId New() => new(Ulid.NewUlid());
}