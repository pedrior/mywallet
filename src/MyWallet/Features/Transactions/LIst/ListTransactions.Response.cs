using MyWallet.Shared.Contracts;

namespace MyWallet.Features.Transactions.List;

public sealed record TransactionResponse
{
    public required Ulid Id { get; init; }

    public required string Type { get; init; }

    public required string Name { get; init; }

    public required string Category { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateOnly Date { get; init; }
}

public sealed record ListTransactionsResponse(
    IEnumerable<TransactionResponse> Items,
    int Page,
    int Limit,
    int Total) : PageResponse<TransactionResponse>(Items, Page, Limit, Total)
{
    public required Ulid WalletId { get; init; }

    public required DateOnly From { get; init; }

    public required DateOnly To { get; init; }
}