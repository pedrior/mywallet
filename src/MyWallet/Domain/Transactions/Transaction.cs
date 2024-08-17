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
    
    public TransactionName Name { get; init; } = null!;

    public Amount Amount { get; private set; } = null!;
    
    public Currency Currency { get; init; } = null!;
    
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
    
    internal void ChangeWallet(WalletId walletId) => WalletId = walletId;

    internal void ChangeCategory(CategoryId categoryId) => CategoryId = categoryId;
}