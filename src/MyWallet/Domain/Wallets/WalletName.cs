using System.Text.RegularExpressions;

namespace MyWallet.Domain.Wallets;

public sealed partial class WalletName : ValueObject
{
    private const int MaxLength = 30;

    public static readonly Error IsEmpty = Error.Validation(description: "Must not be empty.");
    
    public static readonly Error TooLong = Error.Validation(
        description: $"Must be at most {MaxLength} characters long.");

    public static readonly Error Invalid = Error.Validation(
        description: "Must contain only letters, digits, spaces, punctuation and symbols");

    private WalletName()
    {
    }

    public string Value { get; private init; } = null!;

    public static ErrorOr<WalletName> Create(string value) => Validate(value)
        .Then(_ => new WalletName { Value = value });

    public static ErrorOr<Success> Validate(string? value) => ErrorCollection.Empty
        .For(string.IsNullOrWhiteSpace(value), IsEmpty)
        .For(value?.Length > MaxLength, TooLong)
        .For(!string.IsNullOrWhiteSpace(value) && !WalletNameRegex().IsMatch(value), Invalid);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[\p{L}\p{N}\p{P}\p{S} ]+$")]
    private static partial Regex WalletNameRegex();
}