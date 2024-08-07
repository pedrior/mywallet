namespace MyWallet.Features.Categories.Validations;

public static class CategoryValidations
{
    public static IRuleBuilderOptions<T, string> CategoryType<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Must not be empty")
            .Must(name => Domain.Categories.Enums.CategoryType.IsDefined(name))
            .WithMessage($"Must be one of {string.Join(", ", Domain.Categories.Enums.CategoryType.List)}");
    }
}