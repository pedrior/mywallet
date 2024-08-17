using MyWallet.Domain;
using MyWallet.Domain.Categories;
using MyWallet.Features.Categories.Shared;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Categories.Create;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Type)
            .CategoryType();

        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(CategoryName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);
    }
}