namespace MyWallet.Domain.Users;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<ErrorOr<User>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
}