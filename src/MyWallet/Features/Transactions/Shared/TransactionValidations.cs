namespace MyWallet.Features.Transactions.Shared;

public static class TransactionValidations
{
    public static IRuleBuilderOptions<T, string> TransactionType<T>(this IRuleBuilder<T, string> builder)
    {
        return builder
            .NotEmpty()
            .WithMessage("Must not be empty.")
            .Must(name => Domain.Transactions.TransactionType.IsDefined(name))
            .WithMessage($"Must be one of {string.Join(", ", Domain.Transactions.TransactionType.List)}.");
    }
}