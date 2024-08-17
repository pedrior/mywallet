namespace MyWallet.Features.Categories.Shared.Validations;

public static class CategoryValidations
{
    public static IRuleBuilderOptions<T, string> CategoryType<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Must not be empty")
            .Must(name => Domain.Categories.CategoryType.IsDefined(name))
            .WithMessage($"Must be one of {string.Join(", ", Domain.Categories.CategoryType.List)}");
    }
}