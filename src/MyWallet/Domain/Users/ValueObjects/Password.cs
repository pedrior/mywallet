using System.Text.RegularExpressions;
using MyWallet.Shared.Errors;

namespace MyWallet.Domain.Users.ValueObjects;

public sealed partial class Password : ValueObject
{
    private const int MinLength = 6;
    private const int MaxLength = 30;

    public static readonly Error IsEmpty = Error.Validation(description: "Must not be empty.");

    public static readonly Error TooShort = Error.Validation(
        description: $"Must be at least {MinLength} characters long.");

    public static readonly Error TooLong = Error.Validation(
        description: $"Must be at most {MaxLength} characters long.");

    public static readonly Error NoUppercaseLetters = Error.Validation(
        description: "Must contain at least one uppercase letter.");

    public static readonly Error NoLowercaseLetters = Error.Validation(
        description: "Must contain at least one lowercase letter.");

    public static readonly Error NoDigits = Error.Validation(
        description: "Must contain at least one digit.");

    private Password()
    {
    }

    public string Value { get; private init; } = null!;

    public static ErrorOr<Password> Create(string value) =>
        Validate(value)
            .Then(_ => new Password { Value = value });

    public static ErrorOr<Success> Validate(string? value) => ErrorCollection.Empty
        .For(string.IsNullOrWhiteSpace(value), IsEmpty)
        .For(value?.Length < MinLength, TooShort)
        .For(value?.Length > MaxLength, TooLong)
        .For(!string.IsNullOrWhiteSpace(value) && !UppercaseLettersRegex().IsMatch(value), NoUppercaseLetters)
        .For(!string.IsNullOrWhiteSpace(value) && !LowercaseLettersRegex().IsMatch(value), NoLowercaseLetters)
        .For(!string.IsNullOrWhiteSpace(value) && !DigitsRegex().IsMatch(value), NoDigits);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("(?=.*[A-Z])")]
    private static partial Regex UppercaseLettersRegex();

    [GeneratedRegex("(?=.*[a-z])")]
    private static partial Regex LowercaseLettersRegex();

    [GeneratedRegex("(?=.*[0-9])")]
    private static partial Regex DigitsRegex();
}