using MyWallet.Domain.Categories.Enums;
using MyWallet.Domain.Categories.Events;
using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.ValueObjects;

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

    public DateTimeOffset CreatedAt { get; private init; }

    public DateTimeOffset? UpdatedAt { get; private set; }

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
        Color = color,
        CreatedAt = DateTimeOffset.UtcNow
    };

    public void Edit(CategoryName name, Color color)
    {
        Name = name;
        Color = color;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete() => AddEvent(new CategoryDeletedEvent(Id));
}