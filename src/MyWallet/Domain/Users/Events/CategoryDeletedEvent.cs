using MyWallet.Domain.Categories.ValueObjects;

namespace MyWallet.Domain.Users.Events;

public sealed class CategoryDeletedEvent(CategoryId categoryId) : IEvent
{
    public CategoryId CategoryId { get; init; } = categoryId;
}