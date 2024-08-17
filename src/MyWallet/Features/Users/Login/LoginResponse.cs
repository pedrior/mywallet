namespace MyWallet.Features.Users.Login;

public sealed record LoginResponse
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}