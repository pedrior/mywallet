using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.Login;

public sealed record LoginCommand : ICommand<LoginResponse>
{
    public required string Email { get; init; }
    
    public required string Password { get; init; }
}