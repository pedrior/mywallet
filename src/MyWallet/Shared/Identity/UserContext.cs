using MyWallet.Domain.Users;
using MyWallet.Shared.Identity.Extensions;

namespace MyWallet.Shared.Identity;

public sealed class UserContext : IUserContext
{
    private readonly IUserRepository userRepository;
    private bool? isAuthenticated;

    public UserContext(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        
        var principal = httpContextAccessor.HttpContext?.User;
        if (principal is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        Id = principal.GetUserId();
    }

    public required Ulid Id { get; init; }

    public async Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
    {
        if (isAuthenticated.HasValue)
        {
            return isAuthenticated.Value;
        }

        isAuthenticated = await userRepository.ExistsAsync(new UserId(Id), cancellationToken);
        return isAuthenticated.Value;
    }
}