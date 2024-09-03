namespace MyWallet.Features.Users.ChangePassword;

public sealed class ChangePasswordEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/me/change-password", ChangePasswordAsync)
            .RequireAuthorization();

    private static Task<IResult> ChangePasswordAsync(
        ChangePasswordCommand command,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(command, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent(), context);
    }
}