namespace MyWallet.Domain.Wallets.ValueObjects;

public sealed class WalletId(Ulid id) : EntityId(id)
{
    public static WalletId New() => new(Ulid.NewUlid());
}