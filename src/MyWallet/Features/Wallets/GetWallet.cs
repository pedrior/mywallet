using MyWallet.Features.Wallets.Errors;
using MyWallet.Features.Wallets.Security;
using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Security;

namespace MyWallet.Features.Wallets;

public sealed record WalletResponse
{
    public required Ulid Id { get; init; }

    public required string Name { get; init; }

    public required string Color { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset? UpdatedAt { get; init; }
}

public sealed record GetWalletQuery(Ulid WalletId) : IQuery<WalletResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}

public sealed class GetWalletEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("wallets/{id:length(26)}", GetWalletAsync)
            .RequireAuthorization()
            .WithName("GetWallet");

    private static Task<IResult> GetWalletAsync(
        Ulid id,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(new GetWalletQuery(id), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }
}

public sealed class GetWalletAuthorizer : IAuthorizer<GetWalletQuery>
{
    public IEnumerable<IRequirement> GetRequirements(GetWalletQuery query)
    {
        yield return new WalletOwnerRequirement(query.UserId, query.WalletId);
    }
}

public sealed class GetWalletHandler(IDbContext db) : IQueryHandler<GetWalletQuery, WalletResponse>
{
    public async Task<ErrorOr<WalletResponse>> Handle(GetWalletQuery query,
        CancellationToken cancellationToken)
    {
        var response = await db.QuerySingleOrDefaultAsync<WalletResponse>(
            sql: """
                    SELECT w.id,
                           w.name,
                           w.color,
                           w.created_at,
                           w.updated_at
                    FROM wallets w
                    WHERE w.id = @Id
                 """,
            param: new { Id = query.WalletId },
            cancellationToken);

        return response is not null
            ? response
            : WalletErrors.NotFound;
    }
}