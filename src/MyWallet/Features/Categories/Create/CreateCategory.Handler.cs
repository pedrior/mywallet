using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Domain.Users;

namespace MyWallet.Features.Categories.Create;

public sealed class CreateCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<CreateCategoryCommand, CategoryId>
{
    public async Task<ErrorOr<CategoryId>> Handle(CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var type = CategoryType.FromName(command.Type);
        var name = CategoryName.Create(command.Name);
        var color = Color.Create(command.Color);

        var category = Category.Create(
            id: CategoryId.New(),
            userId: new UserId(command.UserId),
            type: type,
            name: name.Value,
            color: color.Value);

        await categoryRepository.AddAsync(category, cancellationToken);
        return category.Id;
    }
}