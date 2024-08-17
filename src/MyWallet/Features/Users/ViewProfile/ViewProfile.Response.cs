namespace MyWallet.Features.Users.ViewProfile;

public sealed record ViewProfileResponse
{
    public required string Name { get; init; }

    public required string Email { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}