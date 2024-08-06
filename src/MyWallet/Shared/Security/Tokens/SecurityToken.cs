namespace MyWallet.Shared.Security.Tokens;

public sealed record SecurityToken(string AccessToken, DateTimeOffset ExpiresAt);