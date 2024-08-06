using System.ComponentModel.DataAnnotations;

namespace MyWallet.Shared.Security.Tokens;

public sealed record SecurityTokenOptions
{
    public const string SectionName = "SecurityToken";
    
    public required string Key { get; init; }

    [Range(5, 43200)]
    public required int ExpiresInMinutes { get; init; }
}