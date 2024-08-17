namespace MyWallet.Features.Users.ChangeEmail;

public sealed record ChangeEmailRequest
{
    public required string NewEmail { get; init; }

    public required string Password { get; init; }
    
    public ChangeEmailCommand ToCommand() => new()
    {
        NewEmail = NewEmail,
        Password = Password
    };
}