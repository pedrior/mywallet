using MyWallet.Shared.Errors;
using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.ChangeEmail;

public sealed class ChangeEmailEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/me/change-email", ChangeEmailAsync)
            .RequireAuthorization();

    private static Task<IResult> ChangeEmailAsync(
        ChangeEmailRequest request,
        ISender sender,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}