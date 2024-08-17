using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Delete;

public sealed class DeleteCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var categoryId = new CategoryId(command.CategoryId);
        var category = await categoryRepository.GetAsync(categoryId, cancellationToken);

        if (category is null)
        {
            return CategoryErrors.NotFound;
        }

        category.Delete();

        await categoryRepository.UpdateAsync(category, cancellationToken);

        return Result.Deleted;
    }
}