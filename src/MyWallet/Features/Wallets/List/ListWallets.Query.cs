using MyWallet.Shared.Contracts;

namespace MyWallet.Features.Wallets.List;

public sealed record ListWalletsQuery : IQuery<PageResponse<WalletSummaryResponse>>, IHaveUser
{
    public required int Page { get; init; }

    public required int Limit { get; init; }

    public Ulid UserId { get; set; }
}