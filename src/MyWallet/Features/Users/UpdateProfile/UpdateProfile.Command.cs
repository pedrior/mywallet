using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.UpdateProfile;

public sealed record UpdateProfileCommand : ICommand<Success>, IHaveUser
{
    public required string Name { get; init; }

    public Ulid UserId { get; set; }
}