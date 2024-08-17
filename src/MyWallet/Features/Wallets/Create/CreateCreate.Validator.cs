using MyWallet.Domain;
using MyWallet.Domain.Wallets;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets.Create;

public sealed class CreateWalletValidator : AbstractValidator<CreateWalletCommand>
{
    public CreateWalletValidator()
    {
        RuleFor(c => c.Name)
            .MustSatisfyErrorValidation(WalletName.Validate);

        RuleFor(c => c.Color)
            .MustSatisfyErrorValidation(Color.Validate);

        RuleFor(c => c.Currency)
            .Currency();
    }
}