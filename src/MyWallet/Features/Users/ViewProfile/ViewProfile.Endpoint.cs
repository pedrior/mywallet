using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.ViewProfile;

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
            .ToResponseAsync(Results.Ok, context);
    }
}