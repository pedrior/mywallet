namespace MyWallet.Features.Users.UpdateProfile;

public sealed class UpdateProfileEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder)
    {
        builder.MapPatch("users/me/profile", UpdateProfileAsync)
            .RequireAuthorization();
    }

    private static Task<IResult> UpdateProfileAsync(
        UpdateProfileRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}