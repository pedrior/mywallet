using MyWallet.Domain.Categories;

namespace MyWallet.Features.Categories.Delete;

public sealed class DeleteCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<DeleteCategoryCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetAsync(
            new CategoryId(command.CategoryId),
            cancellationToken);

        return await category
            .ThenDo(c => c.Delete())
            .ThenDoAsync(c => categoryRepository.UpdateAsync(c, cancellationToken))
            .Then(_ => Result.Deleted);
    }
}