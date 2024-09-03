using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Edit;

public sealed class EditCategoryHandler(ICategoryRepository categoryRepository)
    : ICommandHandler<EditCategoryCommand, Success>
{
    public async Task<ErrorOr<Success>> Handle(EditCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var name = CategoryName.Create(command.Name).Value;
        var color = Color.Create(command.Color).Value;
        
        var category = await categoryRepository.GetAsync(
            new CategoryId(command.CategoryId),
            cancellationToken);

        return await category
            .ThenDo(c => c.Edit(name, color))
            .ThenDoAsync(c => categoryRepository.UpdateAsync(c, cancellationToken))
            .Then(_ => Result.Success);
    }
}