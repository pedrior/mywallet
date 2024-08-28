using MyWallet.Domain.Categories;
using MyWallet.Domain.Wallets;

namespace MyWallet.Domain.Transactions;

public sealed class Transaction : Entity<TransactionId>, IAggregateRoot, IAuditable
{
    private Transaction()
    {
    }

    public override TransactionId Id { get; init; } = null!;

    public WalletId WalletId { get; private set; } = null!;

    public CategoryId CategoryId { get; private set; } = null!;

    public TransactionType Type { get; init; } = null!;

    public TransactionName Name { get; private set; } = null!;

    public Amount Amount { get; private set; } = null!;

    public Currency Currency { get; private set; } = null!;

    public DateOnly Date { get; private set; }

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? UpdatedAt { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    internal static ErrorOr<Transaction> Create(
        TransactionId id,
        WalletId walletId,
        CategoryId categoryId,
        TransactionType type,
        TransactionName name,
        Amount amount,
        Currency currency,
        DateOnly date)
    {
        if (amount == Amount.Zero)
        {
            return TransactionErrors.AmountIsZero;
        }

        return new Transaction
        {
            Id = id,
            WalletId = walletId,
            CategoryId = categoryId,
            Type = type,
            Name = name,
            Amount = amount,
            Currency = currency,
            Date = date
        };
    }

    public ErrorOr<Success> ChangeWallet(Wallet wallet)
    {
        if (wallet.IsArchived)
        {
            return TransactionErrors.WalletIsArchived;
        }

        WalletId = wallet.Id;
        return Result.Success;
    }

    public ErrorOr<Success> ChangeCategory(Category category)
    {
        if (!Type.MatchCategoryType(category.Type))
        {
            return TransactionErrors.CategoryTypeMismatch;
        }

        CategoryId = category.Id;
        return Result.Success;
    }

    public void ChangeName(TransactionName name) => Name = name;

    public void ChangeAmount(Amount amount) => Amount = amount;

    public void ChangeCurrency(Currency currency) => Currency = currency;

    public void ChangeDate(DateOnly date) => Date = date;
}