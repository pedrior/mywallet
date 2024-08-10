namespace MyWallet.Shared.Contracts;

public sealed record PageResponse<T>(IEnumerable<T> Items, int Page, int Limit, int Total)
{
    public static PageResponse<T> Empty(int page, int limit) => new([], page, limit, 0);
    
    public int Page { get; } = Page;

    public int Limit { get; } = Limit;

    public int Total { get; } = Total;

    public IEnumerable<T> Items { get; } = Items;
}