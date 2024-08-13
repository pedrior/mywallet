namespace MyWallet.Domain.Wallets;

public sealed class WalletId(Ulid id) : EntityId(id)
{
    public static WalletId New() => new(Ulid.NewUlid());
}