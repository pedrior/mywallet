namespace MyWallet.Shared.Validations;

public static class CommonValidations
{
    public static IRuleBuilderOptions<TInput, TProperty> MustSatisfyErrorValidation<TInput, TProperty>(
        this IRuleBuilder<TInput, TProperty> builder,
        Func<TProperty, ErrorOr<Success>> validator,
        string? propertyName = null)
    {
        return (IRuleBuilderOptions<TInput, TProperty>)builder.Custom((value, context) =>
        {
            var result = validator(value);
            if (result.IsError)
            {
                result.Errors.ForEach(error => context.AddFailure(
                    propertyName ?? error.Code, error.Description));
            }
        });
    }

    public static IRuleBuilderOptions<T, string?> Currency<T>(this IRuleBuilder<T, string?> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Must(name => MyWallet.Domain.Currency.IsDefined(name!, ignoreCase: true))
            .WithMessage("Must be a supported ISO 4217 currency code.");
    }
    
    public static IRuleBuilderOptions<T, int> PageNumber<T>(this IRuleBuilder<T, int> builder)
    {
        return builder
            .GreaterThan(0)
            .WithMessage("Must be greater than 0.");
    }
    
    public static IRuleBuilderOptions<T, int> PageLimit<T>(this IRuleBuilder<T, int> builder)
    {
        return builder
            .GreaterThan(0)
            .WithMessage("Must be greater than 0.");
    }
}