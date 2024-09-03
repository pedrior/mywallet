namespace MyWallet.Features.Users.Register;

public sealed record RegisterCommand : ICommand<Created>
{
    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}