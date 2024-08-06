using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;
using MyWallet.Shared.Errors;

namespace MyWallet.Features.Users;

public sealed record UserProfileResponse
{
    public required string Name { get; init; }

    public required string Email { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }
}

public sealed record ViewProfileQuery : IQuery<UserProfileResponse>, IHaveUser
{
    public Ulid UserId { get; set; }
}

public sealed class ViewProfileEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("users/me", ViewProfileAsync)
            .RequireAuthorization();

    private static Task<IResult> ViewProfileAsync(
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new ViewProfileQuery(), cancellationToken)
            .ToResponseAsync(Results.Ok);
    }
}

public sealed class ViewProfileHandler(IDbContext dbContext)
    : IQueryHandler<ViewProfileQuery, UserProfileResponse>
{
    public async Task<ErrorOr<UserProfileResponse>> Handle(ViewProfileQuery query,
        CancellationToken cancellationToken)
    {
        var response = await dbContext.QuerySingleOrDefaultAsync<UserProfileResponse>(
            sql: """
                 SELECT u.name, u.email, u.created_at, u.updated_at
                 FROM users u
                 WHERE u.id = @UserId
                 """,
            param: new { query.UserId },
            cancellationToken: cancellationToken);

        return response!;
    }
}