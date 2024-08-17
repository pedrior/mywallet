using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Edit;

public sealed class EditCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<EditCategoryCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(EditCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(
            new CategoryId(command.CategoryId),
            cancellationToken);

        if (category is null)
        {
            return CategoryErrors.NotFound;
        }

        var name = CategoryName.Create(command.Name);
        var color = Color.Create(command.Color);

        category.Edit(
            name: name.Value,
            color: color.Value);

        await categoryRepository.UpdateAsync(category, cancellationToken);

        return Result.Success;
    }
}