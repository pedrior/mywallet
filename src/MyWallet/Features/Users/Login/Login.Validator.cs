using MyWallet.Domain.Users;
using MyWallet.Shared.Validations;

namespace MyWallet.Features.Users.Login;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(c => c.Email)
            .MustSatisfyErrorValidation(Email.Validate);

        RuleFor(c => c.Password)
            .MustSatisfyErrorValidation(Password.Validate);
    }
}