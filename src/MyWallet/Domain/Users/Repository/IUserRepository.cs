using MyWallet.Domain.Users.ValueObjects;

namespace MyWallet.Domain.Users.Repository;

public interface IUserRepository : IRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
}