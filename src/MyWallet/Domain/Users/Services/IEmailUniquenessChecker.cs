namespace MyWallet.Domain.Users.Services;

public interface IEmailUniquenessChecker
{
    Task<bool> IsUniqueAsync(ValueObjects.Email email, CancellationToken cancellationToken);
}