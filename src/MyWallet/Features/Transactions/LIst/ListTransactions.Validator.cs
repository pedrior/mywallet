using MyWallet.Shared.Validations;

namespace MyWallet.Features.Transactions.List;

public sealed class ListTransactionsValidator : AbstractValidator<ListTransactionsQuery>
{
    public ListTransactionsValidator()
    {
        RuleFor(q => q.To)
            .Must((query, to) => query.From <= to)
            .WithMessage("Must be greater than or equal to 'from' date.");
        
        RuleFor(q => q.Page)
            .PageNumber();

        RuleFor(q => q.Limit)
            .PageLimit();
    }
}