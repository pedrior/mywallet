namespace MyWallet.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand : ICommand<Success>, IHaveUser
{
    public required string OldPassword { get; init; }

    public required string NewPassword { get; init; }
    
    public Ulid UserId { get; set; }
}