namespace MyWallet.Features.Users.ChangeEmail;

public sealed record ChangeEmailCommand : ICommand<Success>, IHaveUser
{
    public required string NewEmail { get; init; }

    public required string Password { get; init; }

    public Ulid UserId { get; set; }
}