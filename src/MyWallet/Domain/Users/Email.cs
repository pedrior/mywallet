using System.Text.RegularExpressions;

namespace MyWallet.Domain.Users;

public sealed partial class Email : ValueObject
{
    private const int MaxLength = 254;

    public static readonly Error IsEmpty = Error.Validation(description: "Must not be empty.");

    public static readonly Error TooLong = Error.Validation(
        description: $"Must be at most {MaxLength} characters long.");

    public static readonly Error Invalid = Error.Validation(description: "Must be a valid email address.");

    private Email()
    {
    }

    public string Value { get; private init; } = null!;

    public static ErrorOr<Email> Create(string value) => Validate(value)
        .Then(_ => new Email { Value = value.ToLowerInvariant() });

    public static ErrorOr<Success> Validate(string? value) => ErrorCollection.Empty
        .For(string.IsNullOrWhiteSpace(value), IsEmpty)
        .For(value?.Length > MaxLength, TooLong)
        .For(!string.IsNullOrWhiteSpace(value) && !EmailRegex().IsMatch(value), Invalid);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^(?!.*(?:\.-|-\.))[^@]+@[^\W_](?:[\w-]*[^\W_])?(?:\.[^\W_](?:[\w-]*[^\W_])?)+$")]
    private static partial Regex EmailRegex();
}