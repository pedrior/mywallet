using MyWallet.Domain.Categories.Events;
using MyWallet.Domain.Users;

namespace MyWallet.Domain.Categories;

public sealed class Category : Entity<CategoryId>, IAggregateRoot, IAuditable
{
    private Category()
    {
    }

    public override required CategoryId Id { get; init; }

    public required UserId UserId { get; init; }

    public required CategoryType Type { get; init; }

    public CategoryName Name { get; private set; } = null!;

    public Color Color { get; private set; } = null!;

    // ReSharper disable UnassignedGetOnlyAutoProperty
    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset? UpdatedAt { get; }
    // ReSharper restore UnassignedGetOnlyAutoProperty

    public static Category Create(
        CategoryId id,
        UserId userId,
        CategoryType type,
        CategoryName name,
        Color color) => new()
    {
        Id = id,
        UserId = userId,
        Type = type,
        Name = name,
        Color = color
    };

    public void Edit(CategoryName name, Color color)
    {
        Name = name;
        Color = color;
    }

    public void Delete() => AddEvent(new CategoryDeletedEvent(Id));
}