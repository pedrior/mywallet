using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.ViewProfile;

public sealed record ViewProfileQuery : IQuery<UserProfileResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}