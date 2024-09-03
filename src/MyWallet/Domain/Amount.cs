namespace MyWallet.Domain;

public sealed class Amount : ValueObject
{
    public static readonly Amount Zero = new();

    public static readonly Error IsNegative = Error.Validation(
        description: "Must be greater than or equal to zero.");

    private Amount()
    {
    }

    public decimal Value { get; private init; }

    public static ErrorOr<Amount> Create(decimal value) => Validate(value)
        .Then(_ => new Amount { Value = value });

    public static ErrorOr<Success> Validate(decimal value) => ErrorCollection.Empty
        .For(value < 0m, IsNegative);

    public override string ToString() => Value.ToString("C");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}