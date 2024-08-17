namespace MyWallet.Features.Users.Register;

public sealed record RegisterRequest
{
    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
    
    public RegisterCommand ToCommand() => new()
    {
        Name = Name,
        Email = Email,
        Password = Password
    };
}