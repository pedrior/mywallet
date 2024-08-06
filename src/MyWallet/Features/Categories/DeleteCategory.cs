using MyWallet.Domain.Categories.ValueObjects;
using MyWallet.Domain.Users.Repository;
using MyWallet.Features.Categories.Security;
using MyWallet.Shared.Features;
using MyWallet.Shared.Security;
using MyWallet.Shared.Errors;

namespace MyWallet.Features.Categories;

public sealed record DeleteCategoryCommand : ICommand<Deleted>, IHaveUser
{
    public required Ulid Id { get; init; }

    public Ulid UserId { get; set; }
}

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void Build(IEndpointRouteBuilder builder) =>
        builder.MapDelete("categories/{id:length(26)}", DeleteCategoryAsync)
            .RequireAuthorization();

    private static Task<IResult> DeleteCategoryAsync(
        Ulid id,
        HttpContext context,
        ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new DeleteCategoryCommand { Id = id }, cancellationToken)
            .ToResponseAsync(_ => Results.NoContent());
    }
}

public sealed class DeleteCategoryAuthorizer : IAuthorizer<DeleteCategoryCommand>
{
    public IEnumerable<IRequirement> GetRequirements(DeleteCategoryCommand command)
    {
        yield return new CategoryOwnerRequirement(command.Id);
    }
}

public sealed class DeleteCategoryHandler(IUserRepository userRepository)
    : ICommandHandler<DeleteCategoryCommand, Deleted>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetAsync(new(command.UserId), cancellationToken);
        return await user!.DeleteCategory(new CategoryId(command.Id))
            .ThenDoAsync(_ => userRepository.UpdateAsync(user, cancellationToken));
    }
}