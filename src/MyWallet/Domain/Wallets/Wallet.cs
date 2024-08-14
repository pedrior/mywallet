using MyWallet.Domain.Users;

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

    public Currency Currency { get; private set; } = null!;

    public bool IsArchived { get; private set; }

    public DateTimeOffset? ArchivedAt { get; private set; }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? UpdatedAt { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    public static Wallet Create(
        WalletId id,
        UserId userId,
        WalletName name,
        Color color,
        Currency currency) => new()
    {
        Id = id,
        UserId = userId,
        Name = name,
        Color = color,
        Currency = currency
    };

    public ErrorOr<Success> Archive()
    {
        if (IsArchived)
        {
            return WalletErrors.AlreadyArchived;
        }

        IsArchived = true;
        ArchivedAt = DateTimeOffset.UtcNow;

        return Result.Success;
    }

    public ErrorOr<Success> Unarchive()
    {
        if (!IsArchived)
        {
            return WalletErrors.NotArchived;
        }

        IsArchived = false;
        ArchivedAt = null;
        
        return Result.Success;
    }

    public ErrorOr<Success> Edit(WalletName name, Color color, Currency currency)
    {
        if (IsArchived)
        {
            return WalletErrors.WalletIsArchived;
        }

        Name = name;
        Color = color;
        Currency = currency;

        return Result.Success;
    }
}