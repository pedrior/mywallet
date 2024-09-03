namespace MyWallet.Features.Users.Login;

public sealed class LoginEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/login", LoginAsync);

    private static Task<IResult> LoginAsync(
        LoginRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}