using MyWallet.Domain;
using MyWallet.Domain.Transactions;
using MyWallet.Features.Transactions.Shared;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Transactions.Create;

public sealed class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(c => c.Type)
            .TransactionType();

        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(TransactionName.Validate);

        RuleFor(c => c.Amount)
            .MustSatisfyErrorValidation(Amount.Validate);

        RuleFor(c => c.Currency)
            .Currency();
    }   
}