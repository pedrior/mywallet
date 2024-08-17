namespace MyWallet.Features.Users.UpdateProfile;

public sealed record UpdateProfileRequest
{
    public required string Name { get; init; }
    
    public UpdateProfileCommand ToCommand() => new()
    {
        Name = Name
    };
}