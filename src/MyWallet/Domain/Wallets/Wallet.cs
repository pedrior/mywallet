using MyWallet.Domain.Users.ValueObjects;
using MyWallet.Domain.Wallets.Errors;
using MyWallet.Domain.Wallets.ValueObjects;

namespace MyWallet.Domain.Wallets;

public sealed class Wallet : Entity<WalletId>, IAggregateRoot, IAuditable
{
    private Wallet()
    {
    }

    public override WalletId Id { get; init; } = null!;

    public UserId UserId { get; init; } = null!;

    public WalletName Name { get; private set; } = null!;

    public Color Color { get; private set; } = null!;

    public bool IsArchived { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private init; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Wallet Create(
        WalletId id,
        UserId userId,
        WalletName name,
        Color color) => new()
    {
        Id = id,
        UserId = userId,
        Name = name,
        Color = color,
        CreatedAt = DateTimeOffset.UtcNow
    };

    public ErrorOr<Success> Rename(WalletName name)
    {
        if (IsArchived)
        {
            return WalletErrors.WalletIsArchived;
        }

        Name = name;
        SetUpdateAt();

        return Result.Success;
    }
    
    private void SetUpdateAt() => UpdatedAt = DateTimeOffset.UtcNow;
}