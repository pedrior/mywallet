namespace MyWallet.Shared.Identity;

public interface IUserContext
{ 
    Ulid Id { get; }

    Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default);
}