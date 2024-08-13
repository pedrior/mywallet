using System.Text.RegularExpressions;
using MyWallet.Shared.Errors;

namespace MyWallet.Domain.Users;

public sealed partial class UserName : ValueObject
{
    private const int MinLength = 2;
    private const int MaxLength = 50;

    public static readonly Error IsEmpty = Error.Validation(description: "Must not be empty.");

    public static readonly Error TooShort = Error.Validation(
        description: $"Must be at least {MinLength} characters long.");

    public static readonly Error TooLong = Error.Validation(
        description: $"Must be at most {MaxLength} characters long.");

    public static readonly Error Malformed = Error.Validation(
        description: "Must contain only letters, spaces, dots, apostrophes, " +
                     "and must not start or end with space, dot, or apostrophe.");

    private UserName()
    {
    }

    public string Value { get; private init; } = null!;

    public static ErrorOr<UserName> Create(string value) => Validate(value)
        .Then(_ => new UserName { Value = value });

    public static ErrorOr<Success> Validate(string? value) => ErrorCollection.Empty
        .For(string.IsNullOrWhiteSpace(value), IsEmpty)
        .For(value?.Length < MinLength, TooShort)
        .For(value?.Length > MaxLength, TooLong)
        .For(!string.IsNullOrWhiteSpace(value) && !NameRegex().IsMatch(value), Malformed);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^(?!['.\s])[\p{L}'. ]+(?<!['.\s])$")]
    private static partial Regex NameRegex();
}