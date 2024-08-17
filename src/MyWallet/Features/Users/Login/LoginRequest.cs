namespace MyWallet.Features.Users.Login;

public sealed record LoginRequest
{
    public required string Email { get; init; }
    
    public required string Password { get; init; }
    
    public LoginCommand ToCommand() => new()
    {
        Email = Email,
        Password = Password
    };
}