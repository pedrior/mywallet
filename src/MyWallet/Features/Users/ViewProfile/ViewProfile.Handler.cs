using MyWallet.Shared.Features;
using MyWallet.Shared.Persistence;

namespace MyWallet.Features.Users.ViewProfile;

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