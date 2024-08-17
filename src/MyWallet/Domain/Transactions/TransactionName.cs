using MyWallet.Shared.Errors;

namespace MyWallet.Domain.Transactions;

public sealed class TransactionName : ValueObject
{
    public const int MaxLength = 30;

    public static readonly Error IsEmpty = Error.Validation(description: "Must not be empty.");
    
    public static readonly Error TooLong = Error.Validation(
        description: $"Must be at most {MaxLength} characters long.");

    private TransactionName()
    {
    }

    public string Value { get; private init; } = null!;

    public static ErrorOr<TransactionName> Create(string value) => Validate(value)
        .Then(_ => new TransactionName { Value = value });

    public static ErrorOr<Success> Validate(string? value) => ErrorCollection.Empty
        .For(string.IsNullOrWhiteSpace(value), IsEmpty)
        .For(value?.Length > MaxLength, TooLong);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}