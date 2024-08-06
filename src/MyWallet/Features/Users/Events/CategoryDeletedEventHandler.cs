using MyWallet.Domain.Categories.Repository;
using MyWallet.Domain.Users.Events;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.Events;

public sealed class CategoryDeletedEventHandler(
    ICategoryRepository categoryRepository,
    ILogger<CategoryDeletedEvent> logger) : IEventHandler<CategoryDeletedEvent>
{
    public async Task Handle(CategoryDeletedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling event {@Event}", @event);
        
        await categoryRepository.DeleteAsync(@event.CategoryId, cancellationToken);
    }
}