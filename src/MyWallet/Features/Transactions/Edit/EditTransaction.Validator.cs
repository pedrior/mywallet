using MyWallet.Domain;
using MyWallet.Domain.Transactions;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Transactions.Edit;

public sealed class EditTransactionValidator : AbstractValidator<EditTransactionCommand>
{
    public EditTransactionValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(TransactionName.Validate)
            .Unless(x => x.Name is null);

        RuleFor(c => c.Amount)
            .MustSatisfyErrorValidation(v => Amount.Validate(v!.Value))
            .Unless(c => c.Amount is null);

        RuleFor(c => c.Currency)
            .Currency()
            .Unless(c => c.Currency is null);
    }
}