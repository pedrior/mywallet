using MyWallet.Shared.Features;

namespace MyWallet.Features.Users.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapPost("users/register", RegisterAsync);

    private static Task<IResult> RegisterAsync(
        RegisterRequest request,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(request.ToCommand(), cancellationToken)
            .ToResponseAsync(_ => Results.Created(), context);
    }
}