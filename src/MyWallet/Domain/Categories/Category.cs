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

    public CategoryName Name { get; private set; } = null!;

    public Color Color { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public static Category Create(CategoryId id, UserId userId, CategoryName name, Color color) => new()
    {
        Id = id,
        UserId = userId,
        Name = name,
        Color = color
    };

    public void Edit(CategoryName name, Color color)
    {
        Name = name;
        Color = color;
    }
}