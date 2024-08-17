using MyWallet.Shared.Validations;

namespace MyWallet.Features.Wallets.List;

public sealed class ListWalletsValidator : AbstractValidator<ListWalletsQuery>
{
    public ListWalletsValidator()
    {
        RuleFor(q => q.Page)
            .PageNumber();

        RuleFor(q => q.Limit)
            .PageLimit();
    }
}