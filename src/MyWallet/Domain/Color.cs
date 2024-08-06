using System.Text.RegularExpressions;
using MyWallet.Shared.Errors;

namespace MyWallet.Domain;

public sealed partial class Color : ValueObject
{
    public static readonly Error IsEmpty = Error.Validation(
        description: "Must not be empty.");

    public static readonly Error Invalid = Error.Validation(
        description: "Must be a valid RGB color in hexadecimal format (e.g. #FF0000 or #F00).");

    private Color()
    {
    }

    public string Value { get; private init; } = null!;

    public static ErrorOr<Color> Create(string value) => Validate(value)
        .Then(_ => new Color { Value = value.ToUpperInvariant() });

    public static ErrorOr<Success> Validate(string? value) => ErrorCollection.Empty
        .For(string.IsNullOrWhiteSpace(value), IsEmpty)
        .For(!HexRgbColorRegex().IsMatch(value ?? string.Empty), Invalid);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("^#(([0-9a-fA-F]{2}){3}|([0-9a-fA-F]){3})$")]
    private static partial Regex HexRgbColorRegex();
}