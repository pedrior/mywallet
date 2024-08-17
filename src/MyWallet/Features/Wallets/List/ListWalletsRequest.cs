namespace MyWallet.Features.Wallets.List;

public sealed record ListWalletsRequest
{
    public int? Page { get; init; }

    public int? Limit { get; init; }
    
    public ListWalletsQuery ToQuery() => new()
    {
        Page = Page ?? 1,
        Limit = Limit ?? 10
    };
}