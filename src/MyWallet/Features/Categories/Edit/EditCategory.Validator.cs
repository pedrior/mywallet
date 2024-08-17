using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Categories.Edit;

public sealed class EditCategoryValidator : AbstractValidator<EditCategoryCommand>
{
    public EditCategoryValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(CategoryName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);
    }
}