using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Users.ViewProfile;

public sealed class ViewProfileHandler(IDbContext dbContext)
    : IQueryHandler<ViewProfileQuery, ViewProfileResponse>
{
    public async Task<ErrorOr<ViewProfileResponse>> Handle(ViewProfileQuery query,
        CancellationToken cancellationToken)
    {
        var response = await dbContext.QuerySingleOrDefaultAsync<ViewProfileResponse>(
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