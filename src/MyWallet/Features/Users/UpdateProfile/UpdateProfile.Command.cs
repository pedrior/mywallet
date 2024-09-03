namespace MyWallet.Features.Users.UpdateProfile;

public sealed record UpdateProfileCommand : ICommand<Updated>, IHaveUser
{
    public required string Name { get; init; }

    public Ulid UserId { get; set; }
}