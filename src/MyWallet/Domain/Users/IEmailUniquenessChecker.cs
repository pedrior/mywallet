namespace MyWallet.Domain.Users;

public interface IEmailUniquenessChecker
{
    Task<bool> IsUniqueAsync(Email email, CancellationToken cancellationToken);
}