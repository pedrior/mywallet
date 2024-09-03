namespace MyWallet.Features.Users.ViewProfile;

public sealed record ViewProfileQuery : IQuery<ViewProfileResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}