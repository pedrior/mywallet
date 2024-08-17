using MyWallet.Domain;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets.Edit;

public sealed class EditWalletValidator : AbstractValidator<EditWalletCommand>
{
    public EditWalletValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(WalletName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);

        RuleFor(c => c.Currency)
            .Currency();
    }
}