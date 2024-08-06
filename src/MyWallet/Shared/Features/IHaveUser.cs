namespace MyWallet.Shared.Features;

public interface IHaveUser
{
    Ulid UserId { get; set; }
}