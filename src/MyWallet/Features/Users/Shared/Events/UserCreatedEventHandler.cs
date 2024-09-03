using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;
using MyWallet.Domain.Users.Events;

namespace MyWallet.Features.Users.Shared.Events;

public sealed class UserCreatedEventHandler(
    IDefaultCategoriesProvider defaultCategoriesProvider,
    IUserRepository userRepository,
    ICategoryRepository categoryRepository,
    ILogger<UserCreatedEvent> logger) : IEventHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent e, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling domain event {@Event}", e);
        
        var user = await userRepository.GetAsync(e.UserId, cancellationToken);
        if (user.IsError)
        {
            throw new ApplicationException($"User with ID '{e.UserId}' not found.");
        }

        await CreateDefaultCategoriesForUserAsync(user.Value, cancellationToken);
    }

    private Task CreateDefaultCategoriesForUserAsync(User user, CancellationToken cancellationToken)
    {
        var categories = defaultCategoriesProvider.CreateDefaultCategoriesForUser(user);
        return categoryRepository.AddRangeAsync(categories, cancellationToken);
    }
}