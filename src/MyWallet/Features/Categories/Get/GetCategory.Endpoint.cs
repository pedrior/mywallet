using MyWallet.Shared.Features;

namespace MyWallet.Features.Categories.Get;

public sealed class GetCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapGet("categories/{id:length(26)}", GetCategoryAsync)
            .RequireAuthorization()
            .WithName("GetCategory");

    private static Task<IResult> GetCategoryAsync(
        Ulid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new GetCategoryQuery(id), cancellationToken)
            .ToResponseAsync(Results.Ok, context);
    }
}