using MyWallet.Domain.Categories;
using MyWallet.Domain.Categories.Events;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Events;

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